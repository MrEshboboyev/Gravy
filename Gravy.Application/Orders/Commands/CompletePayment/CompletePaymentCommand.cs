using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Orders.Commands.CompletePayment;

public sealed record CompletePaymentCommand(
    Guid OrderId) : ICommand;

