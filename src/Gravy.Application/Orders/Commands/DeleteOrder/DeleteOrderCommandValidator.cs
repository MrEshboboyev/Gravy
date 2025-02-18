using FluentValidation;

namespace Gravy.Application.Orders.Commands.DeleteOrder;

internal sealed class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        RuleFor(order => order.OrderId).NotEmpty();
    }
}