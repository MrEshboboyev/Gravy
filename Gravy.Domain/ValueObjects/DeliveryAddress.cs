using Gravy.Domain.Errors;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects;

/// <summary>
/// Represents the delivery address for an order.
/// </summary>
public sealed class DeliveryAddress : ValueObject
{
    private DeliveryAddress(
        string street, 
        string city, 
        string state,
        Location location)
    {
        Street = street;
        City = city;
        State = state;
        Location = location;
    }

    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public Location Location { get; } // Contains latitude and longitude

    /// <summary>
    /// Factory method to create a DeliveryAddress.
    /// </summary>
    public static Result<DeliveryAddress> Create(
        string street, 
        string city, 
        string state, 
        double latitude,
        double longitude)
    {
        if (string.IsNullOrWhiteSpace(street))
        {
            return Result.Failure<DeliveryAddress>(
                DomainErrors.DeliveryAddress.StreetEmpty);
        }
        if (string.IsNullOrWhiteSpace(city))
        {
            return Result.Failure<DeliveryAddress>(
                DomainErrors.DeliveryAddress.CityEmpty);
        }
        if (string.IsNullOrWhiteSpace(state))
        {
            return Result.Failure<DeliveryAddress>(
                DomainErrors.DeliveryAddress.StateEmpty);
        }

        // errors handled in Location value object
        var locationResult = Location.Create(latitude, longitude);

        return new DeliveryAddress(street, city, state, locationResult.Value);
    }

    public Location ToLocation() => Location;

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return Location;
    }
}
