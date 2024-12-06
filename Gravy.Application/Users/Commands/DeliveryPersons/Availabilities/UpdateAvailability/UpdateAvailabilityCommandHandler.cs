using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.UpdateAvailability;

internal sealed class UpdateAvailabilityCommandHandler(
    IUserRepository userRepository,
    IDeliveryPersonAvailabilityRepository deliveryPersonAvailabilityRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateAvailabilityCommand>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IDeliveryPersonAvailabilityRepository _deliveryPersonAvailabilityRepository =
        deliveryPersonAvailabilityRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateAvailabilityCommand request,
        CancellationToken cancellationToken)
    {
        var (userId, availabilityId, startTimeUtc, endTimeUtc) = request;

        var user = await _userRepository.GetByIdWithDeliveryPersonDetailsAsync(
            userId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(
                DomainErrors.User.NotFound(userId));
        }

        // fix this converting coming soon
        startTimeUtc = DateTime.SpecifyKind(startTimeUtc, DateTimeKind.Utc);
        endTimeUtc = DateTime.SpecifyKind(endTimeUtc, DateTimeKind.Utc);

        var availabilityResult = user.UpdateDeliveryPersonAvailability(
            availabilityId,
            startTimeUtc,
            endTimeUtc);

        if (availabilityResult.IsFailure)
        {
            return Result.Failure(
                availabilityResult.Error);
        }

        _deliveryPersonAvailabilityRepository.Update(availabilityResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}