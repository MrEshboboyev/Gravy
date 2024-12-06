using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.DeleteAvailability;

internal sealed class DeleteAvailabilityCommandHandler(
    IUserRepository userRepository,
    IDeliveryPersonAvailabilityRepository deliveryPersonAvailabilityRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteAvailabilityCommand>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IDeliveryPersonAvailabilityRepository _deliveryPersonAvailabilityRepository =
        deliveryPersonAvailabilityRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeleteAvailabilityCommand request,
        CancellationToken cancellationToken)
    {
        var (userId, availabilityId) = request;

        var user = await _userRepository.GetByIdWithDeliveryPersonDetailsAsync(
            userId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(
                DomainErrors.User.NotFound(userId));
        }

        var deleteAvailabilityResult = user.DeleteDeliveryPersonAvailability(
            availabilityId);

        if (deleteAvailabilityResult.IsFailure)
        {
            return Result.Failure(
                deleteAvailabilityResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}