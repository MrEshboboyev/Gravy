using Gravy.Domain.Enums;

namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when the status of an order changes.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="OrderId">Identifier of the updated order.</param>
/// <param name="OldStatus">Previous status of the order.</param>
/// <param name="NewStatus">New status of the order.</param>
/// <param name="UpdatedAt">The timestamp when the status was updated.</param>
public sealed record OrderStatusChangedDomainEvent(Guid Id, Guid OrderId, OrderStatus OldStatus, OrderStatus NewStatus, DateTime UpdatedAt)
    : DomainEvent(Id);
