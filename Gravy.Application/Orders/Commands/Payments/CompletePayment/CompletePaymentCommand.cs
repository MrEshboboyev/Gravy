using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Orders.Commands.Payments.CompletePayment;

public sealed record CompletePaymentCommand(
    Guid OrderId) : ICommand;

