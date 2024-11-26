using Gravy.Domain.Errors;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects;

/// <summary>
/// Represents a delivery person's vehicle.
/// </summary>
public sealed class Vehicle : ValueObject
{
    private Vehicle(string type, string licensePlate)
    {
        Type = type;
        LicensePlate = licensePlate;
    }

    public string Type { get; }
    public string LicensePlate { get; }

    /// <summary>
    /// Factory method to create a vehicle.
    /// </summary>
    public static Result<Vehicle> Create(string type, string licensePlate)
    {
        if (string.IsNullOrWhiteSpace(type))
            return Result.Failure<Vehicle>(DomainErrors.Vehicle.TypeEmpty);

        if (string.IsNullOrWhiteSpace(licensePlate)) 
            return Result.Failure<Vehicle>(DomainErrors.Vehicle.LicensePlateEmpty);

        return new Vehicle(type, licensePlate);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Type;
        yield return LicensePlate;
    }
}
