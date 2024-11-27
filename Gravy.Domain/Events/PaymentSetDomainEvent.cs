using Gravy.Domain.Enums;
using Gravy.Domain.Primitives;

namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a payment is set for an order.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="OrderId">Identifier of the order for which the payment is set.</param>
/// <param name="PaymentId">Identifier of the payment.</param>
/// <param name="Amount">Amount of the payment.</param>
/// <param name="Method">Payment method used.</param>
/// <param name="TransactionId">Transaction identifier of the payment.</param>
public sealed record PaymentSetDomainEvent(
    Guid Id,
    Guid OrderId,
    Guid PaymentId,
    decimal Amount,
    PaymentMethod Method,
    string TransactionId
) : DomainEvent(Id);

