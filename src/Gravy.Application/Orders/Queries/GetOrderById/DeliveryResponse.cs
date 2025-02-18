using Gravy.Domain.Enums;

namespace Gravy.Application.Orders.Queries.GetOrderById;

public sealed record DeliveryResponse(
    Guid DeliveryId,
    Guid? DeliveryPersonId,
    DateTime? PickUpTime,
    TimeSpan EstimatedDeliveryTime,
    DateTime? ActualDeliveryTime,
    DeliveryStatus DeliveryStatus,
    DateTime CreatedOnUtc);

