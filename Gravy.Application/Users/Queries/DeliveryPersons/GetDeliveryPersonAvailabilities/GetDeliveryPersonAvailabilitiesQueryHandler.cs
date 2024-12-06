using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;

namespace Gravy.Application.Users.Queries.DeliveryPersons.GetDeliveryPersonAvailabilities;

internal sealed class GetDeliveryPersonAvailabilitiesQueryHandler(
    IUserRepository userRepository)
    : IQueryHandler<GetDeliveryPersonAvailabilitiesQuery, DeliveryPersonAvailabilityListResponse>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result<DeliveryPersonAvailabilityListResponse>> Handle(
        GetDeliveryPersonAvailabilitiesQuery request, CancellationToken cancellationToken)
    {
        // Fetch the user from the repository with delivery person details
        var user = await _userRepository.GetByIdWithDeliveryPersonDetailsAsync
            (request.UserId,
            cancellationToken);

        // If user is not found, return a failure result
        if (user is null)
        {
            return Result.Failure<DeliveryPersonAvailabilityListResponse>(
                DomainErrors.User.NotFound(request.UserId));
        }

        #region Prepare Response
        var availabilities = user.DeliveryPersonDetails.Availabilities;
        #endregion

        // Create and return the DeliveryPersonAvailabilityListResponse object
        var response = new DeliveryPersonAvailabilityListResponse(
            availabilities.Select(
                availability => new DeliveryPersonAvailabilityDetailsResponse(
                    availability.DeliveryPersonId,
                    availability.Id,
                    availability.StartTimeUtc,
                    availability.EndTimeUtc))
            .ToList());

        return response;
    }
}
