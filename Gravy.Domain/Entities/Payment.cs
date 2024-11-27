using Gravy.Domain.Enums;
using Gravy.Domain.Primitives;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents payment details for an order.
/// Part of the Order Aggregate.
/// </summary>
public sealed class Payment : IAuditableEntity
{
    // Constructor
    private Payment(Guid id, Guid orderId, decimal amount, PaymentMethod method,
        string transactionId)
    {
        Id = id;
        OrderId = orderId;
        Amount = amount;
        Method = method;
        TransactionId = transactionId;
        Status = PaymentStatus.Pending;
    }

    private Payment() { }

    // Properties
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string TransactionId { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    /// <summary>
    /// Factory method to create a payment.
    /// </summary>
    public static Payment Create(Guid id, 
        Guid orderId, 
        decimal amount, 
        PaymentMethod method, 
        string transactionId)
    {
        return new Payment(id, orderId, amount, method, transactionId);
    }

    /// <summary>
    /// Marks the payment as completed.
    /// </summary>
    public void MarkAsCompleted()
    {
        Status = PaymentStatus.Completed;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the payment as failed.
    /// </summary>
    public void MarkAsFailed()
    {
        Status = PaymentStatus.Failed;
        ModifiedOnUtc = DateTime.UtcNow;
    }
}
