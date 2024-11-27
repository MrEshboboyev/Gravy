using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a delivery person's specific details in the system.
/// </summary>
public sealed class DeliveryPerson : IAuditableEntity
{
    // Constructor
    private DeliveryPerson(Guid id, Vehicle vehicle)
    {
        Id = id;
        Vehicle = vehicle;
    }

    private DeliveryPerson() { }

    // Properties
    public Guid Id { get; private set; }
    public Vehicle Vehicle { get; private set; }
    public ICollection<Guid> AssignedDeliveries { get; private set; } = [];
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    /// <summary>
    /// Factory method to create a delivery person instance.
    /// </summary>
    public static DeliveryPerson Create(Guid id, Vehicle vehicle)
    {
        return new DeliveryPerson(id, vehicle);
    }
}
