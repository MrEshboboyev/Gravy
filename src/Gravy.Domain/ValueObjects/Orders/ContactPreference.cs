using Gravy.Domain.Errors;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects.Orders;

/// <summary>
/// Represents the contact preference for an order.
/// </summary>
public sealed class ContactPreference : ValueObject
{
    private ContactPreference(string value)
    {
        Value = value;
    }

    public string Value { get; }

    /// <summary>
    /// Factory method to create a ContactPreference.
    /// </summary>
    public static Result<ContactPreference> Create(string contactPreference)
    {
        if (string.IsNullOrWhiteSpace(contactPreference))
        {
            return Result.Failure<ContactPreference>(
                DomainErrors.ContactPreference.Empty);
        }

        return Result.Success(new ContactPreference(contactPreference));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
