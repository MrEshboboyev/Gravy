using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects.Customers;

public sealed class CustomerPhoneNumber : ValueObject
{
    public string Value { get; }

    private CustomerPhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<CustomerPhoneNumber> Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return Result.Failure<CustomerPhoneNumber>("Phone number cannot be empty.");
        }
        if (phoneNumber.Length > 15)
        {
            return Result.Failure<CustomerPhoneNumber>("Phone number cannot be longer than 15 characters.");
        }
        if (!phoneNumber.All(char.IsDigit))
        {
            return Result.Failure<CustomerPhoneNumber>("Phone number can only contain digits.");
        }

        return Result.Success(new CustomerPhoneNumber(phoneNumber));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
