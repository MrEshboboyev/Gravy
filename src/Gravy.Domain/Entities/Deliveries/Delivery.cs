using Gravy.Domain.Enums.Deliveries;
using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using System;
using System.Collections.Generic;
using Gravy.Domain.ValueObjects.Deliveries;

namespace Gravy.Domain.Entities.Deliveries;

/// <summary>
/// Represents a delivery in the system.
/// Acts as the aggregate root for delivery-related data.
/// </summary>
public sealed class Delivery : AggregateRoot, IAuditableEntity
{
    #region Private fields

    private readonly List<DeliveryStatus> _statusUpdates = [];

    #endregion

    #region Constructor

    private Delivery(
        Guid id,
        Guid orderId,
        Guid deliveryPersonId,
        DateTime assignedTime,
        DateTime? pickupTime,
        DateTime estimatedDeliveryTime,
        DateTime? actualDeliveryTime,
        DeliveryLocation currentLocation,
        DeliveryStatusType status,
        DeliveryRoute route) : base(id)
    {
        OrderId = orderId;
        DeliveryPersonId = deliveryPersonId; 
        AssignedTime = assignedTime;
        PickupTime = pickupTime;
        EstimatedDeliveryTime = estimatedDeliveryTime;
        ActualDeliveryTime = actualDeliveryTime;
        CurrentLocation = currentLocation;
        Status = status;
        Route = route;

        // Add initial status update
        AddStatusUpdate(status, "Delivery created and assigned", currentLocation);

        RaiseDomainEvent(new DeliveryCreatedDomainEvent(
            Guid.NewGuid(),
            Id,
            OrderId,
            DeliveryPersonId));
    }

    private Delivery()
    {
    }

    #endregion

    #region Properties

    public Guid OrderId { get; private set; }
    public Guid DeliveryPersonId { get; private set; }
    public DateTime AssignedTime { get; private set; }
    public DateTime? PickupTime { get; private set; }
    public DateTime EstimatedDeliveryTime { get; private set; }
    public DateTime? ActualDeliveryTime { get; private set; }
    public DeliveryLocation CurrentLocation { get; private set; }
    public DeliveryStatusType Status { get; private set; }
    public DeliveryRoute Route { get; private set; }
    public IReadOnlyCollection<DeliveryStatus> StatusUpdates => _statusUpdates.AsReadOnly();
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    #endregion

    #region Factory methods

    /// <summary>
    /// Factory method to create a new delivery.
    /// </summary>
    public static Delivery Create(
        Guid id,
        Guid orderId,
        Guid deliveryPersonId,
        DeliveryLocation currentLocation,
        DateTime estimatedDeliveryTime,
        DeliveryRoute route)
    {
        return new Delivery(
            id,
            orderId,
            deliveryPersonId,
            DateTime.UtcNow,
            null,
            estimatedDeliveryTime,
            null,
            currentLocation,
            DeliveryStatusType.Assigned,
            route);
    }

    #endregion

    #region Own Methods

    /// <summary>
    /// Updates the delivery status and adds a status update entry.
    /// </summary>
    public void UpdateStatus(
        DeliveryStatusType status, 
        string description, 
        DeliveryLocation location)
    {
        Status = status;
        CurrentLocation = location;
        ModifiedOnUtc = DateTime.UtcNow;

        AddStatusUpdate(status, description, location);

        if (status is DeliveryStatusType.PickedUp)
        {
            PickupTime = DateTime.UtcNow;
        }
        else if (status == DeliveryStatusType.Delivered)
        {
            ActualDeliveryTime = DateTime.UtcNow;
        }

        RaiseDomainEvent(new DeliveryStatusUpdatedDomainEvent(
            Guid.NewGuid(),
            Id,
            OrderId,
            Status));
    }

    /// <summary>
    /// Updates the current location of the delivery.
    /// </summary>
    public void UpdateLocation(DeliveryLocation location)
    {
        CurrentLocation = location;
        ModifiedOnUtc = DateTime.UtcNow;

        AddStatusUpdate(Status, "Location updated", location);
    }

    /// <summary>
    /// Updates the estimated delivery time.
    /// </summary>
    public void UpdateEstimatedDeliveryTime(DateTime estimatedDeliveryTime)
    {
        EstimatedDeliveryTime = estimatedDeliveryTime;
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new DeliveryTimeUpdatedDomainEvent(
            Guid.NewGuid(),
            Id,
            OrderId,
            EstimatedDeliveryTime));
    }

    /// <summary>
    /// Reassigns the delivery to a different delivery person.
    /// </summary>
    public void Reassign(Guid newDeliveryPersonId)
    {
        DeliveryPersonId = newDeliveryPersonId;
    }

    #endregion
}