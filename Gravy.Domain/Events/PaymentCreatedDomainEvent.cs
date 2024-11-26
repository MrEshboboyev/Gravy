using Gravy.Domain.Enums;

namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a payment is created.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="PaymentId">Identifier of the payment.</param>
/// <param name="OrderId">Identifier of the associated order.</param>
/// <param name="Amount">Amount of the payment.</param>
/// <param name="PaymentMethod">Method used for the payment.</param>
/// <param name="PaymentStatus">Status of the payment.</param>
public sealed record PaymentCreatedDomainEvent(
    Guid Id, 
    Guid PaymentId, 
    Guid OrderId, 
    decimal Amount,
    PaymentMethod PaymentMethod, 
    PaymentStatus PaymentStatus
    ) : DomainEvent(Id);
