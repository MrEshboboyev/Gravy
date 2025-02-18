using Gravy.Domain.ValueObjects;

namespace Gravy.Presentation.Contracts.Users;

public sealed record AddCustomerDetailsRequest(
    string Street,
    string City,
    string State,
    double Latitude,
    double Longitude);
