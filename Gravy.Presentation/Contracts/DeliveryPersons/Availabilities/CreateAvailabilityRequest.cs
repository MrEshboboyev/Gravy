namespace Gravy.Presentation.Contracts.DeliveryPersons.Availabilities;

public sealed record CreateAvailabilityRequest(
    DateTime StartTimeUtc,
    DateTime EndTimeUtc);