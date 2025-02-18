using Gravy.Domain.Errors;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects;

/// <summary>
/// Represents a delivery person's vehicle.
/// </summary>
public sealed class Vehicle : ValueObject
{
    private static readonly Dictionary<string, double> MaxDeliveryRadiusByType = new()
    {
        { "Pedestrian", 2.0 }, // 2 km
        { "Bicycle", 5.0 },    // 5 km
        { "Car", 10.0 },       // 10 km
        { "Motorbike", 15.0 }, // 15 km
        { "Truck", 20.0 }      // 20 km
    };

    private Vehicle(
        string type, 
        string licensePlate, 
        double maxDeliveryRadius)
    {
        Type = type;
        LicensePlate = licensePlate;
        MaxDeliveryRadius = maxDeliveryRadius;
    }

    private Vehicle() { }

    public string Type { get; }
    public string LicensePlate { get; }
    public double MaxDeliveryRadius { get; } // Radius in kilometers

    /// <summary>
    /// Factory method to create a vehicle.
    /// </summary>
    public static Result<Vehicle> Create(string type, string licensePlate)
    {
        if (string.IsNullOrWhiteSpace(type))
            return Result.Failure<Vehicle>(DomainErrors.Vehicle.TypeEmpty);

        if (string.IsNullOrWhiteSpace(licensePlate))
            return Result.Failure<Vehicle>(DomainErrors.Vehicle.LicensePlateEmpty);

        if (!MaxDeliveryRadiusByType.TryGetValue(type, out var maxDeliveryRadius))
        {
            return Result.Failure<Vehicle>(
                DomainErrors.Vehicle.InvalidType(type)); // Ensure type is valid
        }

        return new Vehicle(type, licensePlate, maxDeliveryRadius);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Type;
        yield return LicensePlate;
        yield return MaxDeliveryRadius;
    }
}
