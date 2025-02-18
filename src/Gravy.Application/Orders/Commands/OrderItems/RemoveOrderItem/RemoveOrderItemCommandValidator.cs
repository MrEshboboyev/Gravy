using FluentValidation;

namespace Gravy.Application.Orders.Commands.OrderItems.RemoveOrderItem;

internal class RemoveOrderItemCommandValidator : AbstractValidator<RemoveOrderItemCommand>
{
    public RemoveOrderItemCommandValidator()
    {
        RuleFor(orderItem => orderItem.OrderId).NotEmpty();

        RuleFor(orderItem => orderItem.OrderItemId).NotEmpty();
    }
}