using Gravy.Domain.Errors;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects.Orders;

/// <summary>
/// Represents the delivery address for an order.
/// </summary>
public sealed class OrderDeliveryAddress : ValueObject
{
    private OrderDeliveryAddress(
        string street,
        string city,
        string state,
        string postalCode,
        string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }

    // Parameterless constructor for EF Core
    private OrderDeliveryAddress() { }

    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string PostalCode { get; }
    public string Country { get; }

    /// <summary>
    /// Factory method to create an OrderDeliveryAddress.
    /// </summary>
    public static Result<OrderDeliveryAddress> Create(
        string street,
        string city,
        string state,
        string postalCode,
        string country)
    {
        if (string.IsNullOrWhiteSpace(street))
        {
            return Result.Failure<OrderDeliveryAddress>(
                DomainErrors.DeliveryAddress.StreetEmpty);
        }
        if (string.IsNullOrWhiteSpace(city))
        {
            return Result.Failure<OrderDeliveryAddress>(
                DomainErrors.DeliveryAddress.CityEmpty);
        }
        if (string.IsNullOrWhiteSpace(state))
        {
            return Result.Failure<OrderDeliveryAddress>(
                DomainErrors.DeliveryAddress.StateEmpty);
        }
        if (string.IsNullOrWhiteSpace(postalCode))
        {
            return Result.Failure<OrderDeliveryAddress>(
                DomainErrors.DeliveryAddress.PostalCodeEmpty);
        }
        if (string.IsNullOrWhiteSpace(country))
        {
            return Result.Failure<OrderDeliveryAddress>(
                DomainErrors.DeliveryAddress.CountryEmpty);
        }

        return Result.Success(new OrderDeliveryAddress(street, city, state, postalCode, country));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return PostalCode;
        yield return Country;
    }
}
