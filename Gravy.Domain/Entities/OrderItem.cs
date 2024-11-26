using Gravy.Domain.Events;
using Gravy.Domain.Primitives;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents an individual item within a customer's order.
/// </summary>
public sealed class OrderItem : AggregateRoot, IAuditableEntity
{
    private OrderItem(Guid id, Guid orderId, Guid menuItemId, int quantity, decimal price) : base(id)
    {
        OrderId = orderId;
        MenuItemId = menuItemId;
        Quantity = quantity;
        Price = price;
    }

    private OrderItem()
    {
    }

    // Properties
    public Guid OrderId { get; private set; }
    public Guid MenuItemId { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    /// <summary>
    /// Factory method to create a new order item.
    /// </summary>
    public static OrderItem Create(
        Guid id,
        Guid orderId,
        Guid menuItemId,
        int quantity,
        decimal price
        )
    {
        var orderItem = new OrderItem(
            id,
            orderId,
            menuItemId,
            quantity, 
            price);

        orderItem.RaiseDomainEvent(new OrderItemCreatedDomainEvent(
            Guid.NewGuid(),
            orderItem.Id,
            orderItem.OrderId,
            orderItem.MenuItemId,
            orderItem.Quantity,
            DateTime.UtcNow));

        return orderItem;
    }

    /// <summary>
    /// Updates the quantity of the order item.
    /// </summary>
    public void UpdateQuantity(int newQuantity)
    {
        Quantity = newQuantity;
        ModifiedOnUtc = DateTime.UtcNow;

        this.RaiseDomainEvent(new OrderItemUpdatedDomainEvent(
            Guid.NewGuid(),
            this.Id,
            this.Quantity,
            DateTime.UtcNow));
    }
}
