namespace Gravy.Presentation.Contracts.DeliveryPersons.Availabilities;

public sealed record CreateAvailabilityRequest(
    DateTime StartTime,
    DateTime EndTime);