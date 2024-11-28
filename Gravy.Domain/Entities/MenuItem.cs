using Gravy.Domain.Enums;
using Gravy.Domain.Primitives;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a menu item offered by a restaurant.
/// Part of the Restaurant aggregate.
/// </summary>
public sealed class MenuItem : IAuditableEntity
{
    // Constructor
    private MenuItem(Guid id, Guid restaurantId, string name, string description, 
        decimal price, Category category, bool isAvailable)
    {
        Id = id;
        RestaurantId = restaurantId;
        Name = name;
        Description = description;
        Price = price;
        Category = category;
        IsAvailable = isAvailable;
    }

    private MenuItem()
    {
    }

    // Properties
    public Guid Id { get; private set; }
    public Guid RestaurantId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public Category Category { get; private set; }
    public bool IsAvailable { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    /// <summary>
    /// Factory method to create a menu item.
    /// </summary>
    public static MenuItem Create(
        Guid id,
        Guid restaurantId,
        string name,
        string description,
        decimal price,
        Category category,
        bool isAvailable)
    {
        return new MenuItem(
            id, 
            restaurantId, 
            name, 
            description, 
            price, 
            category, 
            isAvailable);
    }

    /// <summary>
    /// Updates the menu item's details.
    /// </summary>
    public void UpdateDetails(string name, string description, decimal price, bool isAvailable)
    {
        Name = name;
        Description = description;
        Price = price;
        IsAvailable = isAvailable;
        ModifiedOnUtc = DateTime.UtcNow;
    }
}

