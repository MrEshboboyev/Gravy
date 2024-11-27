namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a new restaurant is created.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="RestaurantId">Identifier of the newly created restaurant.</param>
/// <param name="Name">Name of the created restaurant.</param>
public sealed record RestaurantCreatedDomainEvent(
    Guid Id, 
    Guid RestaurantId, 
    string Name) : DomainEvent(Id);
