namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when delivery-person-specific details are linked to a user.
/// </summary>
/// <param name="Id">Unique identifier for this event instance.</param>
/// <param name="UserId">Identifier of the user linked to the delivery person details.</param>
/// <param name="DeliveryPersonId">Identifier of the delivery person details linked to the user.</param>
public sealed record DeliveryPersonLinkedToUserDomainEvent(
    Guid Id,
    Guid UserId,
    Guid DeliveryPersonId
) : DomainEvent(Id);
