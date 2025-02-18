using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Enums;

namespace Gravy.Application.Orders.Commands.Payments.SetPayment;

public sealed record SetPaymentCommand(
    Guid OrderId,
    PaymentMethod Method,
    string TransactionId) : ICommand;

