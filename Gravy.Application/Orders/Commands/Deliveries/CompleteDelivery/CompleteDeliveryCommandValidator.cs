using FluentValidation;

namespace Gravy.Application.Orders.Commands.Deliveries.CompleteDelivery;

public sealed class CompleteDeliveryCommandValidator : AbstractValidator<CompleteDeliveryCommand>
{
    public CompleteDeliveryCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
    }
}
