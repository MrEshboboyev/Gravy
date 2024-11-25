using Gravy.Domain.Errors;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public const int MaxLength = 50; // Maximum length for an email
    private Email(string value)
    {
        Value = value;
    }
    public string Value { get; }

    /// <summary> 
    /// Creates an Email instance after validating the input. 
    /// </summary> 
    /// <param name="email">The email string to create the Email value object from.</param> 
    /// <returns>A Result object containing the Email value object or an error.</returns>
    public static Result<Email> Create(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result.Failure<Email>(DomainErrors.Email.Empty);
        }
        if (firstName.Split('@').Length != 2)
        {
            return Result.Failure<Email>(DomainErrors.Email.InvalidFormat);
        }
        return new Email(firstName);
    }


    /// <summary> 
    /// Creates an Email instance after validating the input. 
    /// </summary> 
    /// <param name="email">The email string to create the Email value object from.</param> 
    /// <returns>A Result object containing the Email value object or an error.</returns>
    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}