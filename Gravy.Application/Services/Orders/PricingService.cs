using Gravy.Application.Services.Orders.Interfaces;
using Gravy.Domain.Errors;
using Gravy.Domain.Shared;

namespace Gravy.Application.Services.Orders;

/// <summary>
/// Default implementation of the PricingService.
/// </summary>
public class PricingService : IPricingService
{
    public Result<decimal> CalculatePrice(decimal basePrice, int quantity)
    {
        if (quantity <= 0)
            return Result.Failure<decimal>(
                DomainErrors.Price.InvalidQuantity);

        //// Example logic: add a discount for bulk orders
        //decimal discount = 0;
        //if (quantity >= 10) discount = 0.1m; // 10% discount for 10+ items

        //decimal total = basePrice * quantity;
        //return total - (total * discount);

        return Result.Success(basePrice * quantity); 
    }
}