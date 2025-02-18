using FluentValidation;

namespace Gravy.Application.Orders.Commands.OrderItems.AddOrderItem;

internal sealed class AddOrderItemCommandValidator : AbstractValidator<AddOrderItemCommand>
{
    public AddOrderItemCommandValidator()
    {
        RuleFor(orderItem => orderItem.OrderId).NotEmpty();

        RuleFor(orderItem => orderItem.MenuItemId).NotEmpty();

        RuleFor(orderItem => orderItem.Quantity).GreaterThanOrEqualTo(1);
    }
}

