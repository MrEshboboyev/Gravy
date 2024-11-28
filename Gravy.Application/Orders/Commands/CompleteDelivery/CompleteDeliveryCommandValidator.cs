using FluentValidation;

namespace Gravy.Application.Orders.Commands.CompleteDelivery;

public sealed class CompleteDeliveryCommandValidator : AbstractValidator<CompleteDeliveryCommand>
{
    public CompleteDeliveryCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
    }
}
