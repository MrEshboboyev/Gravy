using Gravy.Domain.Errors;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects;

/// <summary> 
/// Represents an email value object. 
/// </summary>
public sealed class LastName : ValueObject
{
    public const int MaxLength = 50; // Maximum length for an email

    private LastName(string value)
    {
        Value = value;
    }
    
    public string Value { get; }
    public static Result<LastName> Create(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result.Failure<LastName>(DomainErrors.LastName.Empty);
        }
        if (lastName.Length > MaxLength)
        {
            return Result.Failure<LastName>(DomainErrors.LastName.TooLong);
        }
        return new LastName(lastName);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}