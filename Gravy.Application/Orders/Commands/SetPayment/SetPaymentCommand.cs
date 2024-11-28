using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Enums;

namespace Gravy.Application.Orders.Commands.SetPayment;

public sealed record SetPaymentCommand(
    Guid OrderId,
    decimal Amount,
    PaymentMethod Method,
    string TransactionId) : ICommand;

