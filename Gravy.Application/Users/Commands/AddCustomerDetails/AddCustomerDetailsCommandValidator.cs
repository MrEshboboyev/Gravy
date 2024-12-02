using FluentValidation;

namespace Gravy.Application.Users.Commands.AddCustomerDetails;

public sealed class AddCustomerDetailsCommandValidator : AbstractValidator<AddCustomerDetailsCommand>
{
    public AddCustomerDetailsCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();

        RuleFor(c => c.DeliveryAddress).NotEmpty();
    }
}

