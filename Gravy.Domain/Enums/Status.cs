namespace Gravy.Domain.Enums;

/// <summary>
/// Represents the status of an order.
/// </summary>
public enum Status
{
    Pending = 10,
    Confirmed = 20,
    Preparing = 30,
    OnTheWay = 40,
    Delivered = 50,
    Cancelled = 60
}
