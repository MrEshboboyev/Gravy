namespace Gravy.Application.Users.Queries.GetDeliveryPersonAvailabilities;

public sealed record DeliveryPersonAvailabilityDetailsResponse(
    Guid DeliveryPersonId,
    Guid AvailabilityId,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc);
