namespace Gravy.Domain.Events;

/// <summary>
/// Domain event triggered when a restaurant is created.
/// </summary>
public sealed record RestaurantCreatedDomainEvent(Guid Id, Guid RestaurantId) : DomainEvent(Id);