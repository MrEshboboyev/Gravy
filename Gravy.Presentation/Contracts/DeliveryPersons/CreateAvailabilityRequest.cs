namespace Gravy.Presentation.Contracts.DeliveryPersons;

public sealed record CreateAvailabilityRequest(
    DateTime StartTimeUtc,
    DateTime EndTimeUtc);