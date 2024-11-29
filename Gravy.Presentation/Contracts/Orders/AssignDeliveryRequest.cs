namespace Gravy.Presentation.Contracts.Orders;

public sealed record AssignDeliveryRequest(
    Guid DeliveryPersonId,
    TimeSpan EstimatedDeliveryTime);