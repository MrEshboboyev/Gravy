using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects.Deliveries;

/// <summary>
/// Represents the current location of a delivery person.
/// </summary>
public sealed class DeliveryPersonLocation : ValueObject
{
    public double Latitude { get; }
    public double Longitude { get; }

    private DeliveryPersonLocation(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static Result<DeliveryPersonLocation> Create(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
        {
            return Result.Failure<DeliveryPersonLocation>("Invalid latitude value.");
        }
        if (longitude < -180 || longitude > 180)
        {
            return Result.Failure<DeliveryPersonLocation>("Invalid longitude value.");
        }

        return Result.Success(new DeliveryPersonLocation(latitude, longitude));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Latitude;
        yield return Longitude;
    }
}