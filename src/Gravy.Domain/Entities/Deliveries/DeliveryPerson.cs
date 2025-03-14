using Gravy.Domain.Enums.Deliveries;
using Gravy.Domain.Errors;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;
using Gravy.Domain.ValueObjects.Deliveries;

namespace Gravy.Domain.Entities.Deliveries;

/// <summary>
/// Represents a delivery person in the system.
/// </summary>
public sealed class DeliveryPerson : Entity
{
    public Guid DeliveryPersonId { get; private set; }
    public string Name { get; private set; }
    public string PhoneNumber { get; private set; }
    public DeliveryPersonLocation CurrentLocation { get; private set; }
    public DeliveryPersonStatus Status { get; private set; }
    public DeliveryPersonTransportType TransportType { get; private set; }
    public decimal CurrentRating { get; private set; }

    private DeliveryPerson(
        Guid deliveryPersonId,
        string name,
        string phoneNumber,
        DeliveryPersonLocation currentLocation,
        DeliveryPersonStatus status,
        DeliveryPersonTransportType transportType,
        decimal currentRating)
    {
        DeliveryPersonId = deliveryPersonId;
        Name = name;
        PhoneNumber = phoneNumber;
        CurrentLocation = currentLocation;
        Status = status;
        TransportType = transportType;
        CurrentRating = currentRating;
    }

    public static DeliveryPerson Create(
        Guid deliveryPersonId,
        string name,
        string phoneNumber,
        DeliveryPersonLocation currentLocation,
        DeliveryPersonStatus status,
        DeliveryPersonTransportType transportType,
        decimal currentRating)
    {
        return new DeliveryPerson(
            deliveryPersonId,
            name,
            phoneNumber,
            currentLocation,
            status,
            transportType,
            currentRating);
    }

    public void UpdateLocation(DeliveryPersonLocation location)
    {
        CurrentLocation = location;
    }

    public void UpdateStatus(DeliveryPersonStatus status)
    {
        Status = status;
    }

    public void UpdateRating(decimal rating)
    {
        CurrentRating = rating;
    }
}
