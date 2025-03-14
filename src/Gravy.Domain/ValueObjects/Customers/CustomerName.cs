using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects.Customers;

public sealed class CustomerName : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }

    private CustomerName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<CustomerName> Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result.Failure<CustomerName>("First name cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result.Failure<CustomerName>("Last name cannot be empty.");
        }
        if (firstName.Length > 50)
        {
            return Result.Failure<CustomerName>("First name cannot be longer than 50 characters.");
        }
        if (lastName.Length > 50)
        {
            return Result.Failure<CustomerName>("Last name cannot be longer than 50 characters.");
        }

        return Result.Success(new CustomerName(firstName, lastName));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return FirstName;
        yield return LastName;
    }
}