namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a new customer is created.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="CustomerId">Identifier of the newly created customer.</param>
/// <param name="UserId">Identifier of the associated user.</param>
public sealed record CustomerCreatedDomainEvent(Guid Id, 
    Guid CustomerId, 
    Guid UserId) 
    : DomainEvent(Id);
