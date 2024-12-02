using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Users.Commands.AddCustomerDetails;

public sealed record AddCustomerDetailsCommand(
    Guid UserId, 
    DeliveryAddress DeliveryAddress) : ICommand;

