using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Users.Commands.AddCustomerDetails;

internal sealed class AddCustomerDetailsCommandHandler(IUserRepository userRepository, 
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<AddCustomerDetailsCommand>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AddCustomerDetailsCommand request, 
        CancellationToken cancellationToken)
    {
        var (userId, deliveryAddrees) = request;

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(
                DomainErrors.User.NotFound(userId));
        }    

        var customerResult = user.AddCustomerDetails(deliveryAddrees);

        if (customerResult.IsFailure)
        {
            return Result.Failure(
                customerResult.Error);
        }

        _customerRepository.Add(customerResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

