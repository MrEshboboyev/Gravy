﻿namespace Gravy.Application.Users.Queries.DeliveryPersons.GetDeliveryPersonAvailabilities;

public sealed record DeliveryPersonAvailabilityListResponse(
    IReadOnlyCollection<DeliveryPersonAvailabilityResponse> DeliveryPersonAvailabilities);
