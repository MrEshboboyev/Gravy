using Gravy.Domain.Enums;
using Gravy.Domain.Events;
using Gravy.Domain.Primitives;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a menu item in the system.
/// </summary>
public sealed class MenuItem : AggregateRoot, IAuditableEntity
{
    // Constructor
    private MenuItem(Guid id, string name, string description, decimal price, Category category,
        bool isAvailable, Guid restaurantId) : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        Category = category;
        IsAvailable = isAvailable;
        RestaurantId = restaurantId;
    }

    private MenuItem()
    {
    }

    // Properties
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public Category Category { get; private set; }
    public bool IsAvailable { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
    public Guid RestaurantId { get; private set; }

    /// <summary>
    /// Creates a new menu item.
    /// </summary>
    public static MenuItem Create(
        Guid id,
        string name,
        string description,
        decimal price,
        Category category,
        bool isAvailable, 
        Guid restaurantId
        )
    {
        var menuItem = new MenuItem(
            id,
            name,
            description,
            price,
            category,
            isAvailable, 
            restaurantId);

        menuItem.RaiseDomainEvent(new MenuItemCreatedDomainEvent(
            Guid.NewGuid(),
            menuItem.Id));

        return menuItem;
    }

    /// <summary>
    /// Marks the menu item as unavailable.
    /// </summary>
    public void MarkAsUnavailable()
    {
        IsAvailable = false;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the price of the menu item.
    /// </summary>
    public void UpdatePrice(decimal newPrice)
    {
        Price = newPrice;
        ModifiedOnUtc = DateTime.UtcNow;
    }
}

