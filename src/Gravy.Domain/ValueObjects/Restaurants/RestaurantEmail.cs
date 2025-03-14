using Gravy.Domain.Errors;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects.Restaurants;

/// <summary>
/// Represents a restaurant's email.
/// </summary>
public sealed class RestaurantEmail : ValueObject
{
    public const int MaxLength = 50; // Maximum length for an email
    private RestaurantEmail(string value)
    {
        Value = value;
    }
    public string Value { get; }

    /// <summary>
    /// Creates a RestaurantEmail instance after validating the input.
    /// </summary>
    /// <param name="email">The email string to create the RestaurantEmail value object from.</param>
    /// <returns>A Result object containing the RestaurantEmail value object or an error.</returns>
    public static Result<RestaurantEmail> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure<RestaurantEmail>(DomainErrors.Email.Empty);
        }
        if (email.Length > MaxLength)
        {
            return Result.Failure<RestaurantEmail>(DomainErrors.Email.TooLong);
        }
        if (!email.Contains("@") || email.Split('@').Length != 2)
        {
            return Result.Failure<RestaurantEmail>(DomainErrors.Email.InvalidFormat);
        }

        return Result.Success(new RestaurantEmail(email));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
