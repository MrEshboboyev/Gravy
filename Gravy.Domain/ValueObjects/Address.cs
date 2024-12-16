using Gravy.Domain.Errors;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects;

public sealed class Address : ValueObject
{
    public const int MaxLength = 50;
    private Address(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Address> Create(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return Result.Failure<Address>(DomainErrors.Address.Empty);
        }
        if (address.Length > MaxLength)
        {
            return Result.Failure<Address>(DomainErrors.Address.TooLong);
        }
        return Result.Success(new Address(address));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}