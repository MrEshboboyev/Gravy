using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects.Deliveries;

namespace Gravy.Domain.Entities.Deliveries;

/// <summary>
/// Represents a delivery route in the system.
/// </summary>
public sealed class DeliveryRoute : Entity
{
    public Guid RouteId { get; private set; }
    public DeliveryRouteLocation StartLocation { get; private set; }
    public DeliveryRouteLocation EndLocation { get; private set; }
    public List<DeliveryRouteLocation> Waypoints { get; private set; }
    public TimeSpan EstimatedDuration { get; private set; }
    public double Distance { get; private set; }

    private DeliveryRoute(
        Guid routeId,
        DeliveryRouteLocation startLocation,
        DeliveryRouteLocation endLocation,
        List<DeliveryRouteLocation> waypoints,
        TimeSpan estimatedDuration,
        double distance)
    {
        RouteId = routeId;
        StartLocation = startLocation;
        EndLocation = endLocation;
        Waypoints = waypoints;
        EstimatedDuration = estimatedDuration;
        Distance = distance;
    }

    public static DeliveryRoute Create(
        Guid routeId,
        DeliveryRouteLocation startLocation,
        DeliveryRouteLocation endLocation,
        List<DeliveryRouteLocation> waypoints,
        TimeSpan estimatedDuration,
        double distance)
    {
        return new DeliveryRoute(
            routeId,
            startLocation,
            endLocation,
            waypoints,
            estimatedDuration,
            distance);
    }

    public void UpdateRoute(
        DeliveryRouteLocation startLocation,
        DeliveryRouteLocation endLocation,
        List<DeliveryRouteLocation> waypoints,
        TimeSpan estimatedDuration,
        double distance)
    {
        StartLocation = startLocation;
        EndLocation = endLocation;
        Waypoints = waypoints;
        EstimatedDuration = estimatedDuration;
        Distance = distance;
    }
}
