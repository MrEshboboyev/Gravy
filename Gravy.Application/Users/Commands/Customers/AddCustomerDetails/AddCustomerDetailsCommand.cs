using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Users.Commands.Customers.AddCustomerDetails;

public sealed record AddCustomerDetailsCommand(
    Guid UserId,
    string Street,
    string City,
    string State,
    string PostalCode) : ICommand;

