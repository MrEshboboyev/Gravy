using Gravy.Domain.Enums;
using Gravy.Domain.Errors;
using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a payment made for an order.
/// </summary>
public sealed class Payment : AggregateRoot, IAuditableEntity
{
    // Constructor
    private Payment(Guid id, Guid orderId, Guid customerId, Money amount, PaymentMethod paymentMethod,
        PaymentStatus paymentStatus, string transactionId) : base(id)
    {
        OrderId = orderId;
        CustomerId = customerId;
        Amount = amount;
        Method = paymentMethod;
        Status = paymentStatus;
        TransactionId = transactionId;
    }

    private Payment() { }

    // Properties
    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string TransactionId { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    /// <summary>
    /// Factory method to create a new payment.
    /// </summary>
    public static Result<Payment> Create(
        Guid id,
        Guid orderId,
        Guid customerId,
        Money amount,
        PaymentMethod paymentMethod,
        string transactionId)
    {
        if (amount == null || amount.Amount <= 0)
            return Result.Failure<Payment>(DomainErrors.Payment.InvalidAmount);

        if (string.IsNullOrWhiteSpace(transactionId))
            return Result.Failure<Payment>(DomainErrors.Payment.TransactionIdEmpty);

        var payment = new Payment(
            id,
            orderId,
            customerId,
            amount,
            paymentMethod,
            PaymentStatus.Pending,
            transactionId);

        payment.RaiseDomainEvent(new PaymentCreatedDomainEvent(
            Guid.NewGuid(),
            payment.Id,
            payment.OrderId,
            payment.Amount.Amount,
            payment.Method,
            payment.Status
            ));

        return payment;
    }

    /// <summary>
    /// Marks the payment as completed and raises a domain event.
    /// </summary>
    public void MarkAsCompleted()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Payment can only be completed if it is pending.");

        Status = PaymentStatus.Completed;
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new PaymentCompletedDomainEvent(
            Guid.NewGuid(), 
            Id, 
            OrderId, 
            Amount.Amount, 
            DateTime.UtcNow));
    }

    /// <summary>
    /// Marks the payment as failed and raises a domain event.
    /// </summary>
    public void MarkAsFailed()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Payment can only be marked as failed if it is pending.");

        Status = PaymentStatus.Failed;
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new PaymentFailedDomainEvent(
            Guid.NewGuid(), 
            Id, 
            OrderId, 
            Amount.Amount, 
            DateTime.UtcNow));
    }
}
