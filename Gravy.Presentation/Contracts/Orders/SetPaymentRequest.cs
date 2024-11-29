using Gravy.Domain.Enums;

namespace Gravy.Presentation.Contracts.Orders;

public sealed record SetPaymentRequest(
    PaymentMethod PaymentMethod,
    decimal Amount, 
    string TransactionId);
