namespace Gravy.Domain.Events;

public sealed record PaymentCompletedDomainEvent(Guid Id, Guid OrderId, DateTime PayedAt)
    : DomainEvent(Id);