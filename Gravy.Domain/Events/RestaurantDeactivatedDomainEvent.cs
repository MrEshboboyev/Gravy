using Gravy.Domain.Primitives;

namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a restaurant is deactivated.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="RestaurantId">Identifier of the deactivated restaurant.</param>
/// <param name="Name">Name of the deactivated restaurant.</param>
public sealed record RestaurantDeactivatedDomainEvent(
    Guid Id, 
    Guid RestaurantId, 
    string Name) : DomainEvent(Id);
