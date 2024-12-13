using Gravy.Application.Users.Queries.DeliveryPersons.GetDeliveryPersonAvailabilities;
using Gravy.Domain.Entities;

namespace Gravy.Application.Users.Queries.Common;

public static class DeliveryPersonAvailabilityResponseFactory
{
    public static DeliveryPersonAvailabilityResponse Create(
        DeliveryPersonAvailability availability)
    {
        return new DeliveryPersonAvailabilityResponse(
            availability.DeliveryPersonId,
            availability.Id,
            availability.StartTimeUtc,
            availability.EndTimeUtc);
    }
}