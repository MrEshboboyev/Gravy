namespace Gravy.Application.Users.Queries.DeliveryPersons.GetDeliveryPersonAvailabilities;

public sealed record DeliveryPersonAvailabilityResponse(
    Guid DeliveryPersonId,
    Guid AvailabilityId,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc);
