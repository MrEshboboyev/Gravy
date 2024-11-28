using FluentValidation;

namespace Gravy.Application.Orders.Commands.CompletePayment;

public sealed class CompletePaymentCommandValidator : AbstractValidator<CompletePaymentCommand>
{
    public CompletePaymentCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
    }
}