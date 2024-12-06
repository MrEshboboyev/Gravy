using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Users.Queries.DeliveryPersons.GetDeliveryPersonAvailabilities;

public sealed record GetDeliveryPersonAvailabilitiesQuery(Guid UserId)
    : IQuery<DeliveryPersonAvailabilityListResponse>;