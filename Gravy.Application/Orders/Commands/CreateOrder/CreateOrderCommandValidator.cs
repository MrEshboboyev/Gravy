using FluentValidation;

namespace Gravy.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(order => order.UserId).NotEmpty();

        RuleFor(order => order.RestaurantId).NotEmpty();
        
        RuleFor(order => order.City).NotEmpty();
        
        RuleFor(order => order.Street).NotEmpty();
    }
}
