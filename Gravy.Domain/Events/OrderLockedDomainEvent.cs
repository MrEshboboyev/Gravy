namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when am order is locked.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="OrderId">Identifier of the locked order.</param>
public sealed record OrderLockedDomainEvent(Guid Id, 
    Guid OrderId) : DomainEvent(Id);
