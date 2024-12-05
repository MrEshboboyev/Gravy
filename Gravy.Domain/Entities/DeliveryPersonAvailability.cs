using Gravy.Domain.Primitives;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents the scheduled availability of a delivery person.
/// </summary>
public sealed class DeliveryPersonAvailability : Entity
{
    // Constructor
    internal DeliveryPersonAvailability(
        Guid id,
        Guid deliveryPersonId, 
        DateTime startTimeUtc, 
        DateTime endTimeUtc)
        : base(id)
    {
        DeliveryPersonId = deliveryPersonId;
        StartTimeUtc = startTimeUtc;
        EndTimeUtc = endTimeUtc;
    }

    private DeliveryPersonAvailability() { }

    // Properties
    public Guid DeliveryPersonId { get; private set; }
    public DateTime StartTimeUtc { get; private set; }
    public DateTime EndTimeUtc { get; private set; }

    /// <summary>
    /// Checks if the availability overlaps with a given time range.
    /// </summary>
    public bool IsAvailableFor(DateTime targetTimeUtc)
    {
        return targetTimeUtc >= StartTimeUtc && targetTimeUtc <= EndTimeUtc;
    }
}
