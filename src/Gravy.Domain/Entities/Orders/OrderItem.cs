using System;
using Gravy.Domain.Primitives;

namespace Gravy.Domain.Entities.Orders;

/// <summary>
/// Represents an item in an order.
/// </summary>
public sealed class OrderItem : Entity
{
    #region Constructors
    private OrderItem(
        Guid id,
        Guid orderId,
        Guid menuItemId,
        int quantity,
        decimal unitPrice,
        string customizations,
        string specialInstructions) : base(id)
    {
        OrderId = orderId;
        MenuItemId = menuItemId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Customizations = customizations;
        SpecialInstructions = specialInstructions;
        ItemSubtotal = quantity * unitPrice;
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
    public decimal UnitPrice { get; private set; }
    public string Customizations { get; private set; }
    public string SpecialInstructions { get; private set; }
    public decimal ItemSubtotal { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ModifiedOnUtc { get; private set; }
    #endregion

    #region Factory Methods
    public static OrderItem Create(
        Guid id,
        Guid orderId,
        Guid menuItemId,
        int quantity,
        decimal unitPrice,
        string customizations,
        string specialInstructions)
    {
        return new OrderItem(
            id,
            orderId,
            menuItemId,
            quantity,
            unitPrice,
            customizations,
            specialInstructions);
    }
    #endregion

    #region Own Methods
    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
        ItemSubtotal = Quantity * UnitPrice;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void UpdateSpecialInstructions(string specialInstructions)
    {
        SpecialInstructions = specialInstructions;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void UpdateCustomizations(string customizations)
    {
        Customizations = customizations;
        ModifiedOnUtc = DateTime.UtcNow;
    }
    #endregion
}