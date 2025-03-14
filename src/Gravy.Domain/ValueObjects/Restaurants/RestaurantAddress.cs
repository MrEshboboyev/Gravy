using Gravy.Domain.Primitives;

namespace Gravy.Domain.ValueObjects.Restaurants;

/// <summary>
/// Represents a restaurant's address.
/// </summary>
public sealed class RestaurantAddress : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string PostalCode { get; }
    public string Country { get; }

    private RestaurantAddress(
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

    public static RestaurantAddress Create(
        string street,
        string city,
        string state,
        string postalCode,
        string country)
    {
        return new RestaurantAddress(
            street,
            city,
            state,
            postalCode,
            country);
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