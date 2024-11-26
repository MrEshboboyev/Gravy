namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when an order item's quantity or details are updated.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="OrderItemId">Identifier of the updated order item.</param>
/// <param name="Quantity">The new quantity of the menu item in the order item.</param>
/// <param name="UpdatedAt">The timestamp when the order item was updated.</param>
public sealed record OrderItemUpdatedDomainEvent(Guid Id, Guid OrderItemId, int Quantity, DateTime UpdatedAt)
    : DomainEvent(Id);
