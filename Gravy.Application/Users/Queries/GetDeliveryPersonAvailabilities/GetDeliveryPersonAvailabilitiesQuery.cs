using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Users.Queries.GetDeliveryPersonAvailabilities;

public sealed record GetDeliveryPersonAvailabilitiesQuery(Guid UserId)
    : IQuery<DeliveryPersonAvailabilityListResponse>;