namespace Gravy.Domain.Enums;

/// <summary>
/// Represents the status of an delivery.
/// </summary>
public enum DeliveryStatus
{
    Pending = 10,
    Assigned = 20,
    PickedUp = 30, 
    Delivered = 40,
    Failed = 50
}
