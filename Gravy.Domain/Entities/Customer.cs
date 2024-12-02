using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a customer's specific details in the system.
/// </summary>
public sealed class Customer : Entity
{
    // Constructor
    internal Customer(
        Guid id, 
        Guid userId, 
        DeliveryAddress defaultDeliveryAddress) 
        : base(id)
    {
        UserId = userId;
        DefaultDeliveryAddress = defaultDeliveryAddress;
    }

    private Customer() { }

    // Properties
    public Guid UserId { get; private set; }
    public DeliveryAddress DefaultDeliveryAddress { get; private set; }
    public ICollection<Guid> FavoriteRestaurants { get; private set; } = [];
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ModifiedOnUtc { get; private set; }

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

    /// <summary>
    /// Updates the customer's details.
    /// </summary>
    public void UpdateDetails(DeliveryAddress newDeliveryAddress)
    {
        DefaultDeliveryAddress = newDeliveryAddress;
        ModifiedOnUtc = DateTime.UtcNow;
    }
}
