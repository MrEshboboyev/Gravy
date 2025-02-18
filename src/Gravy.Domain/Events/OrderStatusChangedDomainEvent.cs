using Gravy.Domain.Enums;

namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when the status of an order changes.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="OrderId">Identifier of the order whose status has changed.</param>
/// <param name="OrderStatus">The new status of the order.</param>
/// <param name="StatusChangedDate">The date and time when the status change occurred.</param>
public sealed record OrderStatusChangedDomainEvent(
    Guid Id,
    Guid OrderId,
    OrderStatus OrderStatus,
    DateTime StatusChangedDate
) : DomainEvent(Id);
