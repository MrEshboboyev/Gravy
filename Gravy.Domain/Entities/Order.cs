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
    public bool IsLocked { get; private set; }
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

    #region Own Methods

    /// <summary>
    /// Locks the order to prevent further modifications.
    /// </summary>
    private void LockOrder()
    {
        IsLocked = true;
        ModifiedOnUtc = DateTime.UtcNow;

        // Raise a domain event if needed
        RaiseDomainEvent(new OrderLockedDomainEvent(
            Guid.NewGuid(),
            Id));
    }

    private void SetStatus(OrderStatus newStatus)
    {
        if (Status == newStatus)
            return;

        Status = newStatus;
        ModifiedOnUtc = DateTime.UtcNow;

        // Optionally raise a domain event when the status changes
        RaiseDomainEvent(new OrderStatusChangedDomainEvent(
            Guid.NewGuid(),
            Id, 
            newStatus, 
            DateTime.UtcNow));
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
        #region Check is Locked (fix this coming soon)
        if (IsLocked)
        {
            return Result.Failure<OrderItem>(
                DomainErrors.Order.OrderIsLocked);
        }
        #endregion

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
        #region Check is Locked (fix this coming soon)
        if (IsLocked)
        {
            return Result.Failure<OrderItem>(
                DomainErrors.Order.OrderIsLocked);
        }
        #endregion

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

    /// <summary>
    /// Remove an item from the order.
    /// </summary>
    public Result RemoveOrderItem(
        Guid orderItemId)
    {
        #region Check is Locked (fix this coming soon)
        if (IsLocked)
        {
            return Result.Failure<OrderItem>(
                DomainErrors.Order.OrderIsLocked);
        }
        #endregion

        #region Get this Order Item
        var orderItem = _orderItems.Find(oi => oi.Id.Equals((orderItemId)));
        if (orderItem is null)
        {
            return Result.Failure<OrderItem>(
                DomainErrors.OrderItem.NotFound(orderItemId));
        }
        #endregion

        #region Remove this Order Item

        _orderItems.Remove(orderItem);

        #endregion

        #region Update this Order
        ModifiedOnUtc = DateTime.UtcNow;
        #endregion

        #region Domain Events
        RaiseDomainEvent(new OrderItemRemovedDomainEvent(
            Guid.NewGuid(),
            Id, // OrderId
            orderItem.Id // OrderItemId
            ));
        #endregion

        return Result.Success();
    }
    #endregion

    #region Delivery related
    public Result<Delivery> CreateDelivery()
    {
        #region Validation: Ensure payment exists and order is locked

        if (_payment is null)
        {
            return Result.Failure<Delivery>(
                DomainErrors.Delivery.PaymentNotSet);
        }

        if (!IsLocked)
        {
            return Result.Failure<Delivery>(
                DomainErrors.Delivery.OrderNotLocked);
        }

        #endregion

        #region Validation: Ensure delivery does not already exist

        if (_delivery is not null)
        {
            return Result.Failure<Delivery>(
                DomainErrors.Delivery.AlreadySet(_delivery.Id));
        }

        #endregion

        #region Create new Delivery for this Order

        _delivery = new Delivery(
            Guid.NewGuid(),
            Id);

        #endregion

        #region Set Status to Preparing
        
        SetStatus(OrderStatus.Preparing);
        
        #endregion

        #region Domain Events

        RaiseDomainEvent(new DeliveryCreatedDomainEvent(
            Guid.NewGuid(),
            Id,
            _delivery.Id, 
            DateTime.UtcNow));

        #endregion

        return Result.Success(_delivery);
    }

    /// <summary>
    /// Assigns a delivery to the order.
    /// </summary>
    public Result<Delivery> AssignDelivery(
    Guid deliveryPersonId,
    TimeSpan estimatedDeliveryTime)
    {
        #region Checking delivery exists

        if (_delivery is null)
        {
            return Result.Failure<Delivery>(
                DomainErrors.Delivery.NotFound(Id));
        }

        #endregion

        #region Checking Delivery Person already assigned

        if (_delivery.DeliveryPersonId is not null)
        {
            return Result.Failure<Delivery>(
                DomainErrors.Delivery.DeliveryPersonAlreadyAssigned);
        }

        #endregion

        // Assign the delivery person
        var assignResult = _delivery.AssignDeliveryPerson(deliveryPersonId);
        if (assignResult.IsFailure)
        {
            return Result.Failure<Delivery>(assignResult.Error);
        }

        #region Set Status to OnTheWay

        SetStatus(OrderStatus.OnTheWay);

        #endregion

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
        DeliveredAt = DateTime.UtcNow;

        #region Set Status to Delivered

        SetStatus(OrderStatus.Delivered);

        #endregion

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

        #region Set Status to Confirmed

        SetStatus(OrderStatus.Confirmed);

        #endregion

        #region Lock the Order
        LockOrder();
        #endregion

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