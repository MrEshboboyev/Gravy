using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a customer who orders food in the system.
/// </summary>
public sealed class Customer : AggregateRoot, IAuditableEntity
{
    // Constructor
    private Customer(Guid id, Guid userId, DeliveryAddress defaultDeliveryAddress) : base(id)
    {
        UserId = userId;
        DefaultDeliveryAddress = defaultDeliveryAddress;
    }

    private Customer() { }

    // Properties
    public Guid UserId { get; private set; } // Link to User
    public DeliveryAddress DefaultDeliveryAddress { get; private set; }
    public ICollection<Guid> FavoriteRestaurants { get; private set; } = [];
    public ICollection<Guid> OrderHistory { get; private set; } = [];
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    /// <summary>
    /// Factory method to create a new customer.
    /// </summary>
    public static Customer Create(Guid id, Guid userId, DeliveryAddress defaultDeliveryAddress)
    {
        var customer = new Customer(
            id,
            userId,
            defaultDeliveryAddress);

        customer.RaiseDomainEvent(new CustomerCreatedDomainEvent(
            Guid.NewGuid(),
            id,
            userId));

        return customer;
    }

    /// <summary>
    /// Adds a restaurant to the customer's favorites and raises a domain event.
    /// </summary>
    public void AddFavoriteRestaurant(Guid restaurantId)
    {
        if (FavoriteRestaurants.Contains(restaurantId))
        {
            return;
        }

        FavoriteRestaurants.Add(restaurantId);
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new FavoriteRestaurantAddedDomainEvent(
            Guid.NewGuid(),
            Id, 
            restaurantId));
    }

    /// <summary>
    /// Removes a restaurant from the customer's favorites and raises a domain event.
    /// </summary>
    public void RemoveFavoriteRestaurant(Guid restaurantId)
    {
        if (!FavoriteRestaurants.Contains(restaurantId))
        {
            return;
        }

        FavoriteRestaurants.Remove(restaurantId);
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new FavoriteRestaurantRemovedDomainEvent(
            Guid.NewGuid(), 
            Id, 
            restaurantId));
    }
}
