using Gravy.Domain.Enums;

namespace Gravy.Application.Orders.Queries.GetOrderById;

public sealed record PaymentResponse(
    Guid PaymentId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    PaymentStatus PaymentStatus,
    string TransactionId,
    DateTime CreatedOnUtc);

