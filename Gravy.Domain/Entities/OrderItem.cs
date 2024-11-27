using Gravy.Domain.Primitives;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents an individual item in an order.
/// Part of the Order Aggregate.
/// </summary>
public sealed class OrderItem : IAuditableEntity
{
    private OrderItem(Guid id, Guid orderId, Guid menuItemId, int quantity, decimal price)
    {
        Id = id;
        OrderId = orderId;
        MenuItemId = menuItemId;
        Quantity = quantity;
        Price = price;
    }

    private OrderItem()
    {
    }

    // Properties
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid MenuItemId { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    /// <summary>
    /// Factory method to create an order item.
    /// </summary>
    public static OrderItem Create(Guid id, 
        Guid orderId, 
        Guid menuItemId, 
        int quantity, 
        decimal price)
    {
        return new OrderItem(id, orderId, menuItemId, quantity, price);
    }
}
