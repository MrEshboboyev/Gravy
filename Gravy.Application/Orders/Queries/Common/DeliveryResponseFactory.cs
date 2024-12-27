using Gravy.Application.Orders.Queries.GetOrderById;
using Gravy.Domain.Entities;

namespace Gravy.Application.Orders.Queries.Common;

public static class DeliveryResponseFactory
{
    public static DeliveryResponse Create(Delivery delivery)
    {
        return new DeliveryResponse(
            delivery.Id,
            delivery.DeliveryPersonId,
            delivery.PickUpTime,
            delivery.EstimatedDeliveryTime,
            delivery.ActualDeliveryTime,
            delivery.DeliveryStatus,
            delivery.CreatedOnUtc);
    }
}