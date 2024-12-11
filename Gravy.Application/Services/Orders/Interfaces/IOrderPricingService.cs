using Gravy.Domain.Shared;

namespace Gravy.Application.Services.Orders.Interfaces;

public interface IOrderPricingService
{
    /// <summary>
    /// Calculates the total price of an order based on its items.
    /// </summary>
    /// <param name="orderId">The ID of the order.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>The calculated total amount.</returns>
    Task<Result<decimal>> CalculateOrderTotalAsync(Guid orderId, CancellationToken cancellationToken);
}