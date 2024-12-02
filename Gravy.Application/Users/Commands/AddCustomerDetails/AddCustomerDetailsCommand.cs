using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Users.Commands.AddCustomerDetails;

public sealed record AddCustomerDetailsCommand(
    Guid UserId,
    string Street,
    string City,
    string State,
    string PostalCode) : ICommand;

