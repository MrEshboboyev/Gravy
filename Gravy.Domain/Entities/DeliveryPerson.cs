using Gravy.Domain.Errors;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a delivery person's specific details in the system.
/// </summary>
public sealed class DeliveryPerson : Entity
{
    #region Private fields
    private readonly List<DeliveryPersonAvailability> _availabilities = [];
    #endregion

    #region Constructors
    internal DeliveryPerson(
        Guid id, 
        Guid userId, 
        Vehicle vehicle) 
        : base(id)
    {
        UserId = userId;
        Vehicle = vehicle;
    }

    private DeliveryPerson() { }
    #endregion

    #region Properties
    public Guid UserId { get; private set; }
    public Vehicle Vehicle { get; private set; }
    public ICollection<Guid> AssignedDeliveries { get; private set; } = [];
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ModifiedOnUtc { get; private set; }
    public bool IsAvailable { get; private set; } = true; // Default to available
    public IReadOnlyCollection<DeliveryPersonAvailability> Availabilities => 
        _availabilities.AsReadOnly();
    #endregion

    #region Own Methods
    /// <summary>
    /// Updates the delivery person's details.
    /// </summary>
    public void UpdateDetails(Vehicle newVehicle)
    {
        Vehicle = newVehicle;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public bool IsAvailableAt(DateTime targetTimeUtc)
    {
        return _availabilities.Any(av => av.IsAvailableFor(targetTimeUtc));
    }
    #endregion

    #region Availability methods (Add/Update/Delete)
    public Result<DeliveryPersonAvailability> AddAvailability(
    DateTime startTimeUtc,
    DateTime endTimeUtc)
    {
        var validationResult = ValidateAvailabilityPeriod(startTimeUtc, endTimeUtc);
        if (validationResult.IsFailure)
        {
            return validationResult;
        }

        // create new availability
        var newAvailability = new DeliveryPersonAvailability(
            Guid.NewGuid(), Id, startTimeUtc, endTimeUtc);

        // adding created availability
        _availabilities.Add(newAvailability);

        // return availability
        return newAvailability;
    }

    public Result<DeliveryPersonAvailability> UpdateAvailability(
        Guid availabilityId,
        DateTime startTimeUtc,
        DateTime endTimeUtc)
    {
        #region get availability
        var availability = _availabilities.Find(a => a.Id == availabilityId);

        if (availability is null)
        {
            return Result.Failure<DeliveryPersonAvailability>(
                DomainErrors.DeliveryPersonAvailability.NotFound(availabilityId));
        }
        #endregion

        var validationResult = ValidateAvailabilityPeriod(startTimeUtc, endTimeUtc, availability.Id);
        if (validationResult.IsFailure)
        {
            return validationResult;
        }

        // update availability details
        availability.UpdateDetails(startTimeUtc, endTimeUtc);

        return availability;
    }

    private Result<DeliveryPersonAvailability> ValidateAvailabilityPeriod(
        DateTime startTimeUtc,
        DateTime endTimeUtc, 
        Guid? availabilityId = null)
    {
        // checking default logic for start and end (for nowUTC)
        if (startTimeUtc < DateTime.UtcNow || endTimeUtc < startTimeUtc)
        {
            return Result.Failure<DeliveryPersonAvailability>(
                DomainErrors.DeliveryPersonAvailability.
                InvalidAvailabilityPeriod(startTimeUtc, endTimeUtc));
        }

        // logic for existing overlapping availability
        var existOverlappingAvailability = _availabilities
            .Any(a => a.Id != availabilityId &&
                ((a.StartTimeUtc <= startTimeUtc && a.EndTimeUtc >= startTimeUtc) || // Starts in the middle of an existing range
                (a.StartTimeUtc <= endTimeUtc && a.EndTimeUtc >= endTimeUtc) ||    // Ends in the middle of an existing range
                (a.StartTimeUtc >= startTimeUtc && a.EndTimeUtc <= endTimeUtc))    // Completely overlaps an existing range
                );

        if (existOverlappingAvailability)
        {
            return Result.Failure<DeliveryPersonAvailability>(
                DomainErrors.DeliveryPersonAvailability
                .OverlappingAvailabilityPeriod(startTimeUtc, endTimeUtc));
        }

        return Result.Success<DeliveryPersonAvailability>(null);
    }
    #endregion
}
