using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a customer's specific details in the system.
/// </summary>
public sealed class Customer : IAuditableEntity
{
    // Constructor
    private Customer(Guid id, DeliveryAddress defaultDeliveryAddress) 
    {
        Id = id;
        DefaultDeliveryAddress = defaultDeliveryAddress;
    }

    private Customer() { }

    // Properties
    public Guid Id { get; private set; }
    public DeliveryAddress DefaultDeliveryAddress { get; private set; }
    public ICollection<Guid> FavoriteRestaurants { get; private set; } = [];
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    /// <summary>
    /// Factory method to create a customer instance.
    /// </summary>
    public static Customer Create(Guid id, DeliveryAddress defaultDeliveryAddress)
    {
        return new Customer(id, defaultDeliveryAddress);
    }

    /// <summary>
    /// Adds a restaurant to the favorites list.
    /// </summary>
    public void AddFavoriteRestaurant(Guid restaurantId)
    {
        if (!FavoriteRestaurants.Contains(restaurantId))
        {
            FavoriteRestaurants.Add(restaurantId);
            ModifiedOnUtc = DateTime.UtcNow;
        }
    }
}
