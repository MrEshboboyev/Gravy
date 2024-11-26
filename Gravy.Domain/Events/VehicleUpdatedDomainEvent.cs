namespace Gravy.Domain.Events;

/// <summary>
/// Event raised when a delivery person's vehicle information is updated.
/// </summary>
/// <param name="Id">Unique identifier for the event instance.</param>
/// <param name="DeliveryPersonId">Identifier of the delivery person.</param>
/// <param name="OldVehicleType">Previous vehicle type.</param>
/// <param name="NewVehicleType">Updated vehicle type.</param>
public sealed record VehicleUpdatedDomainEvent(Guid Id, 
    Guid DeliveryPersonId, 
    string OldVehicleType, 
    string NewVehicleType)
    : DomainEvent(Id);
