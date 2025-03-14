using Gravy.Domain.Enums;
using Gravy.Domain.Enums.Orders;
using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using System;
using System.Collections.Generic;
using Gravy.Domain.ValueObjects.Orders;
using static Gravy.Domain.Errors.DomainErrors;

namespace Gravy.Domain.Entities.Orders;

/// <summary>
/// Represents an order in the system.
/// Acts as the aggregate root for order-related data.
/// </summary>
public sealed class Order : AggregateRoot, IAuditableEntity
{
    #region Private fields
    private readonly List<OrderItem> _orderItems = [];
    #endregion

    #region Constructor
    private Order(
        Guid id,
        Guid customerId,
        Guid restaurantId,
        DateTime orderDate,
        OrderDeliveryAddress deliveryAddress,
        OrderPaymentMethod paymentMethod,
        OrderStatus status,
        OrderPaymentStatus paymentStatus,
        decimal subTotal,
        decimal deliveryFee,
        decimal tax,
        decimal discount,
        decimal totalAmount,
        DeliveryInstructions deliveryInstructions) : base(id)
    {
        CustomerId = customerId;
        RestaurantId = restaurantId;
        OrderDate = orderDate;
        DeliveryAddress = deliveryAddress;
        PaymentMethod = paymentMethod;
        Status = status;
        PaymentStatus = paymentStatus;
        SubTotal = subTotal;
        DeliveryFee = deliveryFee;
        Tax = tax;
        Discount = discount;
        TotalAmount = totalAmount;
        DeliveryInstructions = deliveryInstructions;

        RaiseDomainEvent(new OrderCreatedDomainEvent(
            Guid.NewGuid(),
            Id,
            CustomerId,
            RestaurantId));
    }

    private Order()
    {
    }
    #endregion

    #region Properties

    public Guid CustomerId { get; private set; }
    public Guid RestaurantId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderDeliveryAddress DeliveryAddress { get; private set; }
    public OrderPaymentMethod PaymentMethod { get; private set; }
    public OrderStatus Status { get; private set; }
    public OrderPaymentStatus PaymentStatus { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal DeliveryFee { get; private set; }
    public decimal Tax { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DeliveryInstructions DeliveryInstructions { get; private set; }
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    #endregion

    #region Factory methods

    /// <summary>
    /// Factory method to create a new order.
    /// </summary>
    public static Order Create(
        Guid id,
        Guid customerId,
        Guid restaurantId,
        OrderDeliveryAddress deliveryAddress,
        OrderPaymentMethod paymentMethod,
        decimal subTotal,
        decimal deliveryFee,
        decimal tax,
        decimal discount,
        DeliveryInstructions deliveryInstructions)
    {
        var totalAmount = subTotal + deliveryFee + tax - discount;

        return new Order(
            id,
            customerId,
            restaurantId,
            DateTime.UtcNow,
            deliveryAddress,
            paymentMethod,
            OrderStatus.Created,
            PaymentStatus.Pending,
            subTotal,
            deliveryFee,
            tax,
            discount,
            totalAmount,
            deliveryInstructions);
    }

    #endregion

    #region Own Methods

    /// <summary>
    /// Adds an item to the order.
    /// </summary>
    public void AddOrderItem(OrderItem orderItem)
    {
        _orderItems.Add(orderItem);
        RecalculateTotal();
        ModifiedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes an item from the order.
    /// </summary>
    public void RemoveOrderItem(Guid orderItemId)
    {
        var orderItem = _orderItems.Find(x => x.Id == orderItemId);
        if (orderItem != null)
        {
            _orderItems.Remove(orderItem);
            RecalculateTotal();
            ModifiedOnUtc = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Updates the order status.
    /// </summary>
    public void UpdateStatus(OrderStatus status)
    {
        Status = status;
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new OrderStatusUpdatedDomainEvent(
            Guid.NewGuid(),
            Id,
            Status));
    }

    /// <summary>
    /// Updates the payment status.
    /// </summary>
    public void UpdatePaymentStatus(PaymentStatus paymentStatus)
    {
        PaymentStatus = paymentStatus;
        ModifiedOnUtc = DateTime.UtcNow;

        if (paymentStatus == PaymentStatus.Completed)
        {
            RaiseDomainEvent(new OrderPaidDomainEvent(
                Guid.NewGuid(),
                Id,
                CustomerId,
                TotalAmount));
        }
    }

    /// <summary>
    /// Updates delivery instructions.
    /// </summary>
    public void UpdateDeliveryInstructions(DeliveryInstructions deliveryInstructions)
    {
        DeliveryInstructions = deliveryInstructions;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Recalculates the total amount based on order items.
    /// </summary>
    private void RecalculateTotal()
    {
        SubTotal = 0;
        foreach (var item in _orderItems)
        {
            SubTotal += item.ItemSubtotal;
        }
        TotalAmount = SubTotal + DeliveryFee + Tax - Discount;
    }

    /// <summary>
    /// Cancels the order.
    /// </summary>
    public void Cancel()
    {
        if (Status is OrderStatus.Created or OrderStatus.Confirmed)
        {
            Status = OrderStatus.Cancelled;
            ModifiedOnUtc = DateTime.UtcNow;

            RaiseDomainEvent(new OrderCancelledDomainEvent(
                Guid.NewGuid(),
                Id,
                CustomerId));
        }
    }

    #endregion
}