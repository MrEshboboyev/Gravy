using Gravy.Domain.ValueObjects;

namespace Gravy.Presentation.Contracts.Users;

public sealed record AddDeliveryPersonDetailsRequest(
    string Type,
    string LicensePlate);
