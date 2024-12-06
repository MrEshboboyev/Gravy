using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.AddAvailability;

internal sealed class AddAvailabilityCommandHandler(
    IUserRepository userRepository,
    IDeliveryPersonAvailabilityRepository deliveryPersonAvailabilityRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<AddAvailabilityCommand>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IDeliveryPersonAvailabilityRepository _deliveryPersonAvailabilityRepository =
        deliveryPersonAvailabilityRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AddAvailabilityCommand request,
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

        // fix this converting coming soon
        startTimeUtc = DateTime.SpecifyKind(startTimeUtc, DateTimeKind.Utc);
        endTimeUtc = DateTime.SpecifyKind(endTimeUtc, DateTimeKind.Utc);

        // Check for overlapping availability
        var overlappingEntries = await _deliveryPersonAvailabilityRepository.GetOverlappingAvailabilities(
            user.DeliveryPersonDetails.Id,
            startTimeUtc,
            endTimeUtc);

        if (overlappingEntries.Any())
        {
            return Result.Failure(
                DomainErrors.DeliveryPersonAvailability
                .OverlappingAvailabilityPeriod(startTimeUtc, endTimeUtc));
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