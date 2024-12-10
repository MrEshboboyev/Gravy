using Gravy.Domain.Shared;

namespace Gravy.Application.Services.Orders;

/// <summary>
/// Service for calculating the final price of an order item.
/// </summary>
public interface IPricingService
{
    /// <summary>
    /// Calculates the price for an order item based on menu item details, quantity, and applicable discounts.
    /// </summary>
    /// <param name="basePrice">The base price of the menu item.</param>
    /// <param name="quantity">The quantity of the menu item.</param>
    /// <returns>The calculated final price.</returns>
    Result<decimal> CalculatePrice(decimal basePrice, int quantity);
}