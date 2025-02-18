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
    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure<Email>(DomainErrors.Email.Empty);
        }
        if (email.Split('@').Length != 2)
        {
            return Result.Failure<Email>(DomainErrors.Email.InvalidFormat);
        }

        return Result.Success(new Email(email));
    }


    /// <summary> 
    /// Creates an Email instance after validating the input. 
    /// </summary> 
    /// <returns>A Result object containing the Email value object or an error.</returns>
    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}