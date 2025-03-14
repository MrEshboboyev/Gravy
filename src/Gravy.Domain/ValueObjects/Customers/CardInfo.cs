using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects.Customers;

public sealed class CardInfo : ValueObject
{
    public string CardNumber { get; }
    public string CardHolderName { get; }
    public DateTime ExpiryDate { get; }
    public string CardType { get; }

    private CardInfo(string cardNumber, string cardHolderName, DateTime expiryDate, string cardType)
    {
        CardNumber = cardNumber;
        CardHolderName = cardHolderName;
        ExpiryDate = expiryDate;
        CardType = cardType;
    }

    public static Result<CardInfo> Create(string cardNumber, string cardHolderName, DateTime expiryDate, string cardType)
    {
        if (string.IsNullOrWhiteSpace(cardNumber) || !cardNumber.All(char.IsDigit) || cardNumber.Length != 16)
        {
            return Result.Failure<CardInfo>("Card number must be a 16-digit number.");
        }
        if (string.IsNullOrWhiteSpace(cardHolderName))
        {
            return Result.Failure<CardInfo>("Card holder name cannot be empty.");
        }
        if (expiryDate <= DateTime.UtcNow)
        {
            return Result.Failure<CardInfo>("Expiry date must be in the future.");
        }
        if (string.IsNullOrWhiteSpace(cardType))
        {
            return Result.Failure<CardInfo>("Card type cannot be empty.");
        }

        return Result.Success(new CardInfo(cardNumber, cardHolderName, expiryDate, cardType));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return CardNumber;
        yield return CardHolderName;
        yield return ExpiryDate;
        yield return CardType;
    }
}
