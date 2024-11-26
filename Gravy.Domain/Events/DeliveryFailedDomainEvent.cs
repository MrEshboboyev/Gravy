namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a delivery fails.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="DeliveryId">Identifier of the delivery.</param>
/// <param name="FailedAt">The timestamp when the failure occurred.</param>
public sealed record DeliveryFailedDomainEvent(Guid Id, Guid DeliveryId, DateTime FailedAt)
    : DomainEvent(Id);
