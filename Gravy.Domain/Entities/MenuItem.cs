using Gravy.Domain.Enums;
using Gravy.Domain.Primitives;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a menu item offered by a restaurant.
/// Part of the Restaurant aggregate.
/// </summary>
public sealed class MenuItem : Entity
{
    // Constructor
    internal MenuItem(
        Guid id, 
        Guid restaurantId, 
        string name, 
        string description, 
        decimal price, 
        Category category, 
        bool isAvailable) 
        : base(id)
    {
        RestaurantId = restaurantId;
        Name = name;
        Description = description;
        Price = price;
        Category = category;
        IsAvailable = isAvailable;
        CreatedOnUtc = DateTime.UtcNow;
    }

    private MenuItem()
    {
    }

    // Properties
    public Guid RestaurantId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public Category Category { get; private set; }
    public bool IsAvailable { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ModifiedOnUtc { get; private set; }

    /// <summary>
    /// Updates the menu item's details.
    /// </summary>
    public void UpdateDetails(
        string name, 
        string description, 
        decimal price, 
        Category category, 
        bool isAvailable)
    {
        Name = name;
        Description = description;
        Price = price;
        Category = category;
        IsAvailable = isAvailable;
        ModifiedOnUtc = DateTime.UtcNow;
    }
}

