using Gravy.Domain.Enums.Deliveries;
using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects.Deliveries;

namespace Gravy.Domain.Entities.Deliveries;

/// <summary>
/// Represents the status of a delivery.
/// </summary>
public sealed class DeliveryStatus : Entity
{
    public Guid DeliveryStatusId { get; private set; }
    public DeliveryStatusType Status { get; private set; }
    public string Description { get; private set; }
    public DateTime Timestamp { get; private set; }
    public DeliveryLocation Location { get; private set; }

    private DeliveryStatus(
        Guid deliveryStatusId,
        DeliveryStatusType status,
        string description,
        DateTime timestamp,
        DeliveryLocation location)
    {
        DeliveryStatusId = deliveryStatusId;
        Status = status;
        Description = description;
        Timestamp = timestamp;
        Location = location;
    }

    public static DeliveryStatus Create(
        Guid deliveryStatusId,
        DeliveryStatusType status,
        string description,
        DeliveryLocation location)
    {
        return new DeliveryStatus(
            deliveryStatusId,
            status,
            description,
            DateTime.UtcNow,
            location);
    }

    public void UpdateStatus(
        DeliveryStatusType status,
        string description,
        DeliveryLocation location)
    {
        Status = status;
        Description = description;
        Timestamp = DateTime.UtcNow;
        Location = location;
    }
}