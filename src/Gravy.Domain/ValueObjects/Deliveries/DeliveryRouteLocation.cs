using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects.Deliveries;

/// <summary>
/// Represents a location in a delivery route.
/// </summary>
public sealed class DeliveryRouteLocation : ValueObject
{
    public double Latitude { get; }
    public double Longitude { get; }

    private DeliveryRouteLocation(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static Result<DeliveryRouteLocation> Create(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
        {
            return Result.Failure<DeliveryRouteLocation>("Invalid latitude value.");
        }
        if (longitude is < -180 or > 180)
        {
            return Result.Failure<DeliveryRouteLocation>(
                "Invalid longitude value.");
        }

        return Result.Success(new DeliveryRouteLocation(latitude, longitude));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Latitude;
        yield return Longitude;
    }
}