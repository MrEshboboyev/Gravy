using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Users.Commands.AddDeliveryPersonAvailability;

internal sealed class AddDeliveryPersonAvailabilityCommandHandler(
    IUserRepository userRepository,
    IDeliveryPersonAvailabilityRepository deliveryPersonAvailabilityRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<AddDeliveryPersonAvailabilityCommand>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IDeliveryPersonAvailabilityRepository _deliveryPersonAvailabilityRepository = 
        deliveryPersonAvailabilityRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AddDeliveryPersonAvailabilityCommand request,
        CancellationToken cancellationToken)
    {
        var (userId, startTimeUtc, endTimeUtc) = request;

        var user = await _userRepository.GetByIdWithDeliveryPersonDetailsAsync(
            userId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(
                DomainErrors.User.NotFound(userId));
        }

        var availabilityResult = user.AddDeliveryPersonAvailability(startTimeUtc,
            endTimeUtc);

        if (availabilityResult.IsFailure)
        {
            return Result.Failure(
                availabilityResult.Error);
        }

        _deliveryPersonAvailabilityRepository.Add(availabilityResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}