using Gravy.Domain.Errors;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects;

/// <summary>
/// Represents the delivery address for an order.
/// </summary>
public sealed class DeliveryAddress : ValueObject
{
    private DeliveryAddress(string street, string city, string state, string postalCode)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
    }

    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string PostalCode { get; }

    /// <summary>
    /// Factory method to create a DeliveryAddress.
    /// </summary>
    public static Result<DeliveryAddress> Create(string street, string city, string state, string postalCode)
    {
        if (string.IsNullOrWhiteSpace(street))
        {
            return Result.Failure<DeliveryAddress>(DomainErrors.DeliveryAddress.StreetEmpty);
        }
        if (string.IsNullOrWhiteSpace(city))
        {
            return Result.Failure<DeliveryAddress>(DomainErrors.DeliveryAddress.CityEmpty);
        }
        if (string.IsNullOrWhiteSpace(state))
        {
            return Result.Failure<DeliveryAddress>(DomainErrors.DeliveryAddress.StateEmpty);
        }
        if (string.IsNullOrWhiteSpace(postalCode))
        {
            return Result.Failure<DeliveryAddress>(DomainErrors.DeliveryAddress.PostalCodeEmpty);
        }

        return new DeliveryAddress(street, city, state, postalCode);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return PostalCode;
    }
}
