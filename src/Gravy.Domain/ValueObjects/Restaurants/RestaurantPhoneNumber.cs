using Gravy.Domain.Errors;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects.Restaurants;

/// <summary>
/// Represents a restaurant's phone number.
/// </summary>
public sealed class RestaurantPhoneNumber : ValueObject
{
    public const int MaxLength = 15; // Maximum length for a phone number
    private RestaurantPhoneNumber(string value)
    {
        Value = value;
    }
    public string Value { get; }

    /// <summary>
    /// Creates a RestaurantPhoneNumber instance after validating the input.
    /// </summary>
    /// <param name="phoneNumber">The phone number string to create the RestaurantPhoneNumber value object from.</param>
    /// <returns>A Result object containing the RestaurantPhoneNumber value object or an error.</returns>
    public static Result<RestaurantPhoneNumber> Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return Result.Failure<RestaurantPhoneNumber>(DomainErrors.PhoneNumber.Empty);
        }
        if (phoneNumber.Length > MaxLength)
        {
            return Result.Failure<RestaurantPhoneNumber>(DomainErrors.PhoneNumber.TooLong);
        }
        if (!phoneNumber.All(char.IsDigit))
        {
            return Result.Failure<RestaurantPhoneNumber>(DomainErrors.PhoneNumber.InvalidFormat);
        }

        return Result.Success(new RestaurantPhoneNumber(phoneNumber));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
