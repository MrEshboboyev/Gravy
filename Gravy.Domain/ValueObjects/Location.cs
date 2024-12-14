using Gravy.Domain.Errors;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects;

/// <summary>
/// Represents a geographical location with latitude and longitude.
/// </summary>
public sealed class Location : ValueObject
{
    #region Constructors
    private Location(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
    #endregion

    #region Properties
    public double Latitude { get; }
    public double Longitude { get; }
    #endregion

    #region Methods

    /// <summary>
    /// Factory method to create a Location.
    /// </summary>
    public static Result<Location> Create(
        double latitude,
        double longitude)
    {
        #region Checking fields is valid

        // checking latitude
        if (latitude is < -90.0 or > 90.0)
        {
            return Result.Failure<Location>(
                DomainErrors.Location.InvalidLatitude);
        }

        // checking longitude
        if (longitude is < -180.0 or > 180.0)
        {
            return Result.Failure<Location>(
                DomainErrors.Location.InvalidLongitude);
        }

        #endregion

        return Result.Success(new Location(latitude, longitude));
    }

    /// <summary>
    /// Calculates the distance (in kilometers) between this location and 
    /// another location using the Haversine formula.
    /// </summary>
    /// <param name="other">The other location.</param>
    /// <returns>The distance in kilometers.</returns>
    public double CalculateDistance(Location other)
    {
        const double EarthRadiusKm = 6371.0;

        double latitudeDiffRadians = DegreesToRadians(other.Latitude - Latitude);
        double longitudeDiffRadians = DegreesToRadians(other.Longitude - Longitude);

        double latitude1Radians = DegreesToRadians(Latitude);
        double latitude2Radians = DegreesToRadians(other.Latitude);

        double a = Math.Sin(latitudeDiffRadians / 2) * Math.Sin(latitudeDiffRadians / 2) +
                   Math.Cos(latitude1Radians) * Math.Cos(latitude2Radians) *
                   Math.Sin(longitudeDiffRadians / 2) * Math.Sin(longitudeDiffRadians / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180.0);
    }

    /// <summary>
    /// Checks equality based on latitude and longitude.
    /// </summary>
    /// <param name="other">The other location.</param>
    /// <returns>True if both locations are equal, false otherwise.</returns>
    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Latitude;
        yield return Longitude;
    }

    #endregion
}
