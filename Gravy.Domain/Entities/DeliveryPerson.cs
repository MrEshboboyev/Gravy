using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a delivery person's specific details in the system.
/// </summary>
public sealed class DeliveryPerson : Entity
{
    // Constructor
    internal DeliveryPerson(
        Guid id, 
        Guid userId, 
        Vehicle vehicle) 
        : base(id)
    {
        UserId = userId;
        Vehicle = vehicle;
    }

    private DeliveryPerson() { }

    // Properties
    public Guid UserId { get; private set; }
    public Vehicle Vehicle { get; private set; }
    public ICollection<Guid> AssignedDeliveries { get; private set; } = [];
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ModifiedOnUtc { get; private set; }

    /// <summary>
    /// Updates the delivery person's details.
    /// </summary>
    public void UpdateDetails(Vehicle newVehicle)
    {
        Vehicle = newVehicle;
        ModifiedOnUtc = DateTime.UtcNow;
    }
}
