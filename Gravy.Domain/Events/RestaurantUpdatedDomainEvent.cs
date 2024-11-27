using Gravy.Domain.Primitives;

namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a restaurant's details are updated.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="RestaurantId">Identifier of the updated restaurant.</param>
/// <param name="Name">Updated name of the restaurant.</param>
public sealed record RestaurantUpdatedDomainEvent(
    Guid Id, 
    Guid RestaurantId, 
    string Name) : DomainEvent(Id);
