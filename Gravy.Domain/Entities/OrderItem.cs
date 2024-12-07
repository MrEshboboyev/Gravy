using Gravy.Domain.Primitives;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents an individual item in an order.
/// Part of the Order Aggregate.
/// </summary>
public sealed class OrderItem : Entity
{
    #region Constructors
    internal OrderItem(
        Guid id, 
        Guid orderId, 
        Guid menuItemId, 
        int quantity, 
        decimal price) : base(id)
    {
        OrderId = orderId;
        MenuItemId = menuItemId;
        Quantity = quantity;
        Price = price;
        CreatedOnUtc = DateTime.UtcNow;
    }

    private OrderItem()
    {
    }
    #endregion

    #region Properties
    public Guid OrderId { get; private set; }
    public Guid MenuItemId { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ModifiedOnUtc { get; private set; }
    #endregion

    #region Own methods
    /// <summary>
    /// Updates the order item's details.
    /// </summary>
    public void UpdateDetails(
        int quantity, 
        decimal price)
    {
        Quantity = quantity;
        Price = price;
        ModifiedOnUtc = DateTime.UtcNow;
    }
    #endregion
}
