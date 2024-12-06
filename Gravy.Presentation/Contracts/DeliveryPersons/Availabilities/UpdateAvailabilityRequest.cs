namespace Gravy.Presentation.Contracts.DeliveryPersons.Availabilities;

public sealed record UpdateAvailabilityRequest(
    DateTime StartTimeUtc,
    DateTime EndTimeUtc);