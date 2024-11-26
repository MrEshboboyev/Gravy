namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a delivery person is created.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="DeliveryPersonId">Identifier of the newly created delivery person.</param>
/// <param name="UserId">Identifier of the associated user.</param>
public sealed record DeliveryPersonCreatedDomainEvent(Guid Id,
    Guid DeliveryPersonId, 
    Guid UserId) 
    : DomainEvent(Id);
