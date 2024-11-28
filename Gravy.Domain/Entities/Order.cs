using Gravy.Domain.Enums;
using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a customer's order.
/// Acts as the root entity of the Order Aggregate.
/// </summary>
public sealed class Order : AggregateRoot, IAuditableEntity
{
    private readonly List<OrderItem> _orderItems = [];
    private Delivery? _delivery;
    private Payment? _payment;

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

    // Properties
    public Guid CustomerId { get; private set; }
    public Guid RestaurantId { get; private set; }
    public DeliveryAddress DeliveryAddress { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime PlacedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public Delivery? Delivery => _delivery;
    public Payment? Payment => _payment;

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

    /// <summary>
    /// Adds an item to the order.
    /// </summary>
    public void AddOrderItem(Guid menuItemId, int quantity, decimal price)
    {
        var orderItem = OrderItem.Create(Guid.NewGuid(), Id, menuItemId, quantity, price);
        _orderItems.Add(orderItem);
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new OrderItemAddedDomainEvent(
            Guid.NewGuid(),
            Id, // OrderId
            orderItem.Id, // OrderItemId
            menuItemId,
            quantity,
            price));
    }


    /// <summary>
    /// Assigns a delivery to the order.
    /// </summary>
    public void AssignDelivery(Guid deliveryId, Guid deliveryPersonId, TimeSpan estimatedDeliveryTime)
    {
        if (_delivery != null)
            throw new InvalidOperationException("Delivery is already assigned to this order.");

        _delivery = Delivery.Create(deliveryId, Id, deliveryPersonId, estimatedDeliveryTime);
        Status = OrderStatus.OnTheWay;
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new DeliveryAssignedDomainEvent(
            Guid.NewGuid(), 
            deliveryId, 
            deliveryPersonId, 
            DateTime.UtcNow));
    }

    /// <summary>
    /// Sets the payment for the order.
    /// </summary>
    public void SetPayment(Guid paymentId, decimal amount, PaymentMethod method, string transactionId)
    {
        if (_payment != null)
            throw new InvalidOperationException("Payment is already set for this order.");

        _payment = Payment.Create(paymentId, Id, amount, method, transactionId);
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new PaymentSetDomainEvent(
            Guid.NewGuid(),
            Id, // OrderId
            paymentId,
            amount,
            method,
            transactionId));
    }

    /// <summary>
    /// Marks the payment as completed
    /// </summary>
    public void CompletePayment()
    {
        if (_payment == null)
            throw new InvalidOperationException("No delivery is setted to this order.");

        _payment.MarkAsCompleted();
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new PaymentCompletedDomainEvent(
            Guid.NewGuid(),
            Id, // OrderId
            DateTime.UtcNow));
    }

    /// <summary>
    /// Marks the delivery as completed and updates the order status.
    /// </summary>
    public void CompleteDelivery()
    {
        if (_delivery == null)
            throw new InvalidOperationException("No delivery is assigned to this order.");

        _delivery.MarkAsDelivered();
        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new OrderDeliveredDomainEvent(
            Guid.NewGuid(), 
            Id, 
            DeliveredAt.Value));
    }
}