using Gravy.Application.Orders.Queries.GetOrderById;
using Gravy.Domain.Entities;

namespace Gravy.Application.Orders.Queries.Common;

public static class PaymentResponseFactory
{
    public static PaymentResponse Create(Payment payment)
    {
        return new PaymentResponse(
            payment.Id,
            payment.Amount,
            payment.Method,
            payment.Status,
            payment.TransactionId,
            payment.CreatedOnUtc);
    }
}