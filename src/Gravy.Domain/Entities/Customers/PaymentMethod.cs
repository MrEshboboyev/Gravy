using Gravy.Domain.Enums.Customers;
using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects.Customers;

namespace Gravy.Domain.Entities.Customers;

/// <summary>
/// Represents a payment method associated with a customer.
/// </summary>
public sealed class PaymentMethod : Entity
{
    #region Constructors

    private PaymentMethod(
        Guid id,
        Guid customerId,
        PaymentType type,
        CardInfo cardInfo,
        bool isDefault,
        PaymentMethodStatus status) : base(id)
    {
        CustomerId = customerId;
        Type = type;
        CardInfo = cardInfo;
        IsDefault = isDefault;
        Status = status;
        CreatedOnUtc = DateTime.UtcNow;
    }

    // ORM
    private PaymentMethod()
    {
    }

    #endregion

    #region Properties

    public Guid CustomerId { get; private set; }
    public PaymentType Type { get; private set; }
    public CardInfo CardInfo { get; private set; }
    public bool IsDefault { get; private set; }
    public PaymentMethodStatus Status { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ModifiedOnUtc { get; private set; }
    
    #endregion

    #region Factory Methods

    public static PaymentMethod Create(
        Guid id,
        Guid customerId,
        PaymentType type,
        CardInfo cardInfo,
        bool isDefault,
        PaymentMethodStatus status)
    {
        return new PaymentMethod(
            id,
            customerId,
            type,
            cardInfo,
            isDefault,
            status);
    }

    #endregion

    #region Own Methods

    public void Update(
        CardInfo cardInfo,
        bool isDefault)
    {
        CardInfo = cardInfo;
        IsDefault = isDefault;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void UpdateStatus(PaymentMethodStatus status)
    {
        Status = status;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void UnsetDefault()
    {
        IsDefault = false;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    #endregion
}