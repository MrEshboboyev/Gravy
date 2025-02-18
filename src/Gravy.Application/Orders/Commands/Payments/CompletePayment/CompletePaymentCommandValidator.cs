using FluentValidation;

namespace Gravy.Application.Orders.Commands.Payments.CompletePayment;

internal sealed class CompletePaymentCommandValidator : AbstractValidator<CompletePaymentCommand>
{
    public CompletePaymentCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
    }
}