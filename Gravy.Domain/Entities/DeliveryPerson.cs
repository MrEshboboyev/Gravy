using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a delivery person responsible for delivering orders.
/// </summary>
public sealed class DeliveryPerson : AggregateRoot, IAuditableEntity
{
    // Constructor
    private DeliveryPerson(Guid id, Guid userId, Vehicle vehicle) : base(id)
    {
        UserId = userId;
        Vehicle = vehicle;
    }

    private DeliveryPerson() { }

    // Properties
    public Guid UserId { get; private set; } // Link to User
    public Vehicle Vehicle { get; private set; }
    public ICollection<Guid> AssignedDeliveries { get; private set; } = [];
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    /// <summary>
    /// Factory method to create a new delivery person.
    /// </summary>
    public static DeliveryPerson Create(
        Guid id, 
        Guid userId, 
        Vehicle vehicle)
    {
        var deliveryPerson = new DeliveryPerson(
            id,
            userId,
            vehicle);

        deliveryPerson.RaiseDomainEvent(new DeliveryPersonCreatedDomainEvent(
            Guid.NewGuid(),
            id,
            userId));

        return deliveryPerson;
    }

    /// <summary>
    /// Assigns a delivery to the delivery person and raises a domain event.
    /// </summary>
    public void AssignDelivery(Guid deliveryId)
    {
        if (AssignedDeliveries.Contains(deliveryId))
        {
            return;
        }
        AssignedDeliveries.Add(deliveryId);
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new DeliveryAssignedDomainEvent(
            Guid.NewGuid(),
            Id,
            deliveryId,
            DateTime.UtcNow));
    }

    /// <summary>
    /// Updates the delivery person's vehicle and raises a domain event.
    /// </summary>
    public void UpdateVehicle(Vehicle newVehicle)
    {
        var oldVehicleType = Vehicle.Type;
        Vehicle = newVehicle;
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new VehicleUpdatedDomainEvent(
            Guid.NewGuid(), 
            Id, 
            oldVehicleType, 
            newVehicle.Type));
    }
}
