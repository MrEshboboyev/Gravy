﻿namespace Gravy.Application.Users.Queries.GetDeliveryPersonAvailabilities;

public sealed record DeliveryPersonAvailabilityListResponse(
    IReadOnlyCollection<DeliveryPersonAvailabilityDetailsResponse> DeliveryPersonAvailabilities);
