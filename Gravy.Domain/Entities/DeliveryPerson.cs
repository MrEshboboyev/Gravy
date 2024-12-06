using Gravy.Domain.Errors;
using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a delivery person's specific details in the system.
/// </summary>
public sealed class DeliveryPerson : Entity
{
    private readonly List<DeliveryPersonAvailability> _availabilities = [];

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
    public bool IsAvailable { get; private set; } = true; // Default to available
    public IReadOnlyCollection<DeliveryPersonAvailability> Availabilities => 
        _availabilities.AsReadOnly();

    /// <summary>
    /// Updates the delivery person's details.
    /// </summary>
    public void UpdateDetails(Vehicle newVehicle)
    {
        Vehicle = newVehicle;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public DeliveryPersonAvailability AddAvailability(DateTime startTimeUtc, DateTime endTimeUtc)
    {
        var newAvailability = new DeliveryPersonAvailability(
            Guid.NewGuid(), Id, startTimeUtc, endTimeUtc);

        _availabilities.Add(newAvailability);

        return newAvailability;
    }

    public bool IsAvailableAt(DateTime targetTimeUtc)
    {
        return _availabilities.Any(av => av.IsAvailableFor(targetTimeUtc));
    }
}
