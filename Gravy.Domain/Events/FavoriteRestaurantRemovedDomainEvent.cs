namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a restaurant is removed from a customer's favorites.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="CustomerId">Identifier of the customer.</param>
/// <param name="RestaurantId">Identifier of the removed restaurant.</param>
public sealed record FavoriteRestaurantRemovedDomainEvent(Guid Id, 
    Guid CustomerId, 
    Guid RestaurantId) 
    : DomainEvent(Id);
