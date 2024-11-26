namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a payment is completed.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="PaymentId">Identifier of the completed payment.</param>
/// <param name="OrderId">Identifier of the associated order.</param>
/// <param name="Amount">Amount of the payment.</param>
/// <param name="CompletedAt">Timestamp when the payment was completed.</param>
public sealed record PaymentCompletedDomainEvent(
    Guid Id, 
    Guid PaymentId, 
    Guid OrderId, 
    decimal Amount, 
    DateTime CompletedAt
    ) : DomainEvent(Id);
