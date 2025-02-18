namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when customer-specific details are linked to a user.
/// </summary>
/// <param name="Id">Unique identifier for this event instance.</param>
/// <param name="UserId">Identifier of the user linked to the customer details.</param>
/// <param name="CustomerId">Identifier of the customer details linked to the user.</param>
public sealed record CustomerLinkedToUserDomainEvent(
    Guid Id,
    Guid UserId,
    Guid CustomerId
) : DomainEvent(Id);
