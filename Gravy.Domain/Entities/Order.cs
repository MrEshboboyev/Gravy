using Gravy.Domain.Enums;
using Gravy.Domain.Errors;
using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a customer's order.
/// Acts as the root entity of the Order Aggregate.
/// </summary>
public sealed class Order : AggregateRoot, IAuditableEntity
{
    #region Private fields
    private readonly List<OrderItem> _orderItems = [];
    private Delivery _delivery;
    private Payment _payment;
    #endregion

    #region Constructors
    private Order(Guid id, 
        Guid customerId, 
        Guid restaurantId, 
        DeliveryAddress deliveryAddress) : base(id)
    {
        CustomerId = customerId;
        RestaurantId = restaurantId;
        DeliveryAddress = deliveryAddress;
        Status = OrderStatus.Pending;
        PlacedAt = DateTime.UtcNow;

        RaiseDomainEvent(new OrderCreatedDomainEvent(
            Guid.NewGuid(),
            Id, 
            CustomerId, 
            PlacedAt));
    }

    private Order(){ }
    #endregion

    #region Properties
    public Guid CustomerId { get; private set; }
    public Guid RestaurantId { get; private set; }
    public DeliveryAddress DeliveryAddress { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime PlacedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public Delivery Delivery => _delivery;
    public Payment Payment => _payment;
    #endregion

    #region Factory methods
    /// <summary>
    /// Factory method to create a new order.
    /// </summary>
    public static Order Create(
        Guid id,
        Guid customerId, 
        Guid restaurantId, 
        DeliveryAddress deliveryAddress
        )
    {
        return new Order(id, 
            customerId, 
            restaurantId, 
            deliveryAddress);
    }
    #endregion

    #region Order-Item related
    /// <summary>
    /// Adds an item to the order.
    /// </summary>
    public Result<OrderItem> AddOrderItem(
        Guid menuItemId, 
        int quantity, 
        decimal price)
    {
        #region Create the new Order Item
        var orderItem = new OrderItem(
            Guid.NewGuid(), 
            Id, 
            menuItemId, 
            quantity, 
            price);
        #endregion

        #region Add and update this Order
        _orderItems.Add(orderItem);
        ModifiedOnUtc = DateTime.UtcNow;
        #endregion

        #region Domain Events
        RaiseDomainEvent(new OrderItemAddedDomainEvent(
            Guid.NewGuid(),
            Id, // OrderId
            orderItem.Id, // OrderItemId
            menuItemId,
            quantity,
            price));
        #endregion

        return orderItem;   
    }

    /// <summary>
    /// Update an item in the order.
    /// </summary>
    public Result<OrderItem> UpdateOrderItem(
        Guid orderItemId,
        int quantity,
        decimal price)
    {
        #region Get this Order Item
        var orderItem = _orderItems.Find(oi => oi.Id.Equals((orderItemId)));
        if (orderItem is null)
        {
            return Result.Failure<OrderItem>(
                DomainErrors.OrderItem.NotFound(orderItemId));
        }
        #endregion

        #region Update this Order Item 
        var updatedOrderDetailsResult = orderItem.UpdateDetails(quantity, price);
        if (updatedOrderDetailsResult.IsFailure)
        {
            return Result.Failure<OrderItem>(
                updatedOrderDetailsResult.Error);
        }
        #endregion

        #region Update this Order
        ModifiedOnUtc = DateTime.UtcNow;
        #endregion

        #region Domain Events
        RaiseDomainEvent(new OrderItemUpdatedDomainEvent(
            Guid.NewGuid(),
            Id, // OrderId
            orderItem.Id, // OrderItemId
            quantity,
            price));
        #endregion

        return Result.Success(orderItem);
    }
    #endregion

    #region Delivery related
    public Result<Delivery> CreateDelivery()
    {
        if (_delivery is not null)
        {
            return Result.Failure<Delivery>(
                DomainErrors.Delivery.AlreadySet(_delivery.Id));
        }

        _delivery = new Delivery(
            Guid.NewGuid(),
            Id);

        RaiseDomainEvent(new DeliveryCreatedDomainEvent(
            Guid.NewGuid(),
            Id,
            _delivery.Id, 
            DateTime.UtcNow));

        return _delivery;
    }

    /// <summary>
    /// Assigns a delivery to the order.
    /// </summary>
    public Result<Delivery> AssignDelivery(
    Guid deliveryPersonId,
    TimeSpan estimatedDeliveryTime)
    {
        // Ensure a delivery exists
        if (_delivery is null)
        {
            return Result.Failure<Delivery>(
                DomainErrors.Delivery.NotFound(Id));
        }

        // Assign the delivery person
        var assignResult = _delivery.AssignDeliveryPerson(deliveryPersonId);
        if (assignResult.IsFailure)
        {
            return Result.Failure<Delivery>(assignResult.Error);
        }

        // Update order status and timestamp
        Status = OrderStatus.OnTheWay;
        ModifiedOnUtc = DateTime.UtcNow;

        // Raise a domain event
        RaiseDomainEvent(new DeliveryAssignedDomainEvent(
            Guid.NewGuid(),
            _delivery.Id,
            deliveryPersonId,
            DateTime.UtcNow));

        return Result.Success(_delivery);
    }

    /// <summary>
    /// Marks the delivery as completed and updates the order status.
    /// </summary>
    public Result<Delivery> CompleteDelivery()
    {
        if (_delivery is null)
        {
            return Result.Failure<Delivery>(
                DomainErrors.Delivery.NotAssigned(Id));
        }

        _delivery.MarkAsDelivered();

        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;

        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new OrderDeliveredDomainEvent(
            Guid.NewGuid(),
            Id,
            DeliveredAt.Value));

        return _delivery;
    }
    #endregion

    #region Payment related
    /// <summary>
    /// Sets the payment for the order.
    /// </summary>
    public Result<Payment> SetPayment(
    decimal amount,
    PaymentMethod method,
    string transactionId)
    {
        // Ensure a payment has not already been set
        if (_payment is not null)
        {
            return Result.Failure<Payment>(
                DomainErrors.Payment.AlreadySet(_payment.Id));
        }

        // Validate the transaction ID
        if (string.IsNullOrWhiteSpace(transactionId))
        {
            return Result.Failure<Payment>(
                DomainErrors.Payment.TransactionIdEmpty);
        }

        // Create the payment object
        _payment = new Payment(
            Guid.NewGuid(),
            Id,
            amount,
            method,
            transactionId);

        // Update the order's modified timestamp
        ModifiedOnUtc = DateTime.UtcNow;

        // Raise a domain event for setting the payment
        RaiseDomainEvent(new PaymentSetDomainEvent(
            Guid.NewGuid(),
            Id, // OrderId
            _payment.Id,
            amount,
            method,
            transactionId));

        return Result.Success(_payment);
    }


    /// <summary>
    /// Marks the payment as completed
    /// </summary>
    public Result<Payment> CompletePayment()
    {
        if (_payment is null)
        {
            return Result.Failure<Payment>(
                DomainErrors.Payment.AlreadySet(_payment.Id));
        }

        _payment.MarkAsCompleted();

        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new PaymentCompletedDomainEvent(
            Guid.NewGuid(),
            Id, // OrderId
            DateTime.UtcNow));

        return _payment;
    }
    #endregion
}