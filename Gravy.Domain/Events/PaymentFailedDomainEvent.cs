namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a payment fails.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="PaymentId">Identifier of the failed payment.</param>
/// <param name="OrderId">Identifier of the associated order.</param>
/// <param name="Amount">Amount of the payment.</param>
/// <param name="FailedAt">Timestamp when the payment failed.</param>
public sealed record PaymentFailedDomainEvent(
    Guid Id, 
    Guid PaymentId, 
    Guid OrderId, 
    decimal Amount, 
    DateTime FailedAt
    ) : DomainEvent(Id);
