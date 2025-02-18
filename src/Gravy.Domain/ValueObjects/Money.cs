using Gravy.Domain.Errors;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects;

/// <summary>
/// Represents a monetary value.
/// </summary>
public sealed class Money : ValueObject
{
    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }
    public string Currency { get; }

    /// <summary>
    /// Factory method to create a Money instance.
    /// </summary>
    public static Result<Money> Create(decimal amount, string currency)
    {
        if (amount <= 0)
            return Result.Failure<Money>(DomainErrors.Money.InvalidAmount);

        if (string.IsNullOrWhiteSpace(currency))
            return Result.Failure<Money>(DomainErrors.Money.CurrencyEmpty);

        return new Money(amount, currency);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Amount;
        yield return Currency;
    }
}
