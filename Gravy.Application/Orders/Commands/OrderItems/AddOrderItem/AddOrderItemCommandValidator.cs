using FluentValidation;

namespace Gravy.Application.Orders.Commands.OrderItems.AddOrderItem;

public sealed class AddOrderItemCommandValidator : AbstractValidator<AddOrderItemCommand>
{
    public AddOrderItemCommandValidator()
    {
        RuleFor(orderItem => orderItem.OrderId).NotEmpty();

        RuleFor(orderItem => orderItem.MenuItemId).NotEmpty();

        RuleFor(orderItem => orderItem.Quantity).GreaterThanOrEqualTo(1);

        RuleFor(orderItem => orderItem.Price).GreaterThan(0);
    }
}

