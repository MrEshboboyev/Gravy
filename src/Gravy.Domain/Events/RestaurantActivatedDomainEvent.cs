namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a restaurant is activated.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="RestaurantId">Identifier of the activated restaurant.</param>
/// <param name="Name">Name of the deactivated restaurant.</param>
public sealed record RestaurantActivatedDomainEvent(
    Guid Id, 
    Guid RestaurantId, 
    string Name) : DomainEvent(Id);
