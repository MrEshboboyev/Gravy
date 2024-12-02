using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Users.Commands.AddDeliveryPersonDetails;

internal sealed class AddDeliveryPersonDetailsCommandHandler(IUserRepository userRepository,
    IDeliveryPersonRepository deliveryPersonRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<AddDeliveryPersonDetailsCommand>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IDeliveryPersonRepository _deliveryPersonRepository = deliveryPersonRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AddDeliveryPersonDetailsCommand request,
        CancellationToken cancellationToken)
    {
        var (userId, deliveryAddrees) = request;

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(
                DomainErrors.User.NotFound(userId));
        }

        var deliveryPersonResult = user.AddDeliveryPersonDetails(deliveryAddrees);

        if (deliveryPersonResult.IsFailure)
        {
            return Result.Failure(
                deliveryPersonResult.Error);
        }

        _deliveryPersonRepository.Add(deliveryPersonResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}