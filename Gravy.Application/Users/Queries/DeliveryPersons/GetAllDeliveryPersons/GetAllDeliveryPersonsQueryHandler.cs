using Gravy.Application.Abstractions.Messaging;
using Gravy.Application.Users.Queries.GetUserById;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Users.Queries.DeliveryPersons.GetAllDeliveryPersons;

internal sealed class GetAllDeliveryPersonsQueryHandler(
    IDeliveryPersonRepository deliveryPersonRepository) : IQueryHandler<GetAllDeliveryPersonsQuery, DeliveryPersonListResponse>
{
    private readonly IDeliveryPersonRepository _deliveryPersonRepository
        = deliveryPersonRepository;

    public async Task<Result<DeliveryPersonListResponse>> Handle(GetAllDeliveryPersonsQuery request,
        CancellationToken cancellationToken)
    {
        var allDeliveryPersons = await _deliveryPersonRepository.GetAllAsync(cancellationToken);

        var response = new DeliveryPersonListResponse(
            allDeliveryPersons
                .Select(deliveryPerson => new DeliveryPersonDetailsResponse(
                    deliveryPerson.Id,
                    deliveryPerson.UserId,
                    PrepareVehicle(deliveryPerson.Vehicle),
                    deliveryPerson.AssignedDeliveries,
                    deliveryPerson.CreatedOnUtc))
                .ToList());

        return response;
    }

    static string PrepareVehicle(Vehicle vehicle) =>
               $"Type : {vehicle.Type} | " +
               $"License Plate : {vehicle.LicensePlate}";
}

