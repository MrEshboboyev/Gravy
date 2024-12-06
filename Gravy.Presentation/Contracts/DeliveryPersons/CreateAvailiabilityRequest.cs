namespace Gravy.Presentation.Contracts.DeliveryPersons;

public sealed record CreateAvailiabilityRequest(
    DateTime StartTimeUtc,
    DateTime EndTimeUtc);