using Gravy.Domain.Enums;
using Gravy.Domain.Primitives;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents payment details for an order.
/// Part of the Order Aggregate.
/// </summary>
public sealed class Payment : Entity
{
    #region Constructors
    internal Payment(
        Guid id, 
        Guid orderId, 
        decimal amount, 
        PaymentMethod method,
        string transactionId) : base(id)
    {
        OrderId = orderId;
        Amount = amount;
        Method = method;
        TransactionId = transactionId;
        Status = PaymentStatus.Pending;
        CreatedOnUtc = DateTime.UtcNow;
    }

    private Payment() { }
    #endregion

    #region Properties
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string TransactionId { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ModifiedOnUtc { get; private set; }
    #endregion

    #region Own methods

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

    #endregion
}
