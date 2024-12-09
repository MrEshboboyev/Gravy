namespace Gravy.Presentation.Helpers;

internal static class LocationHelpers
{
    private static readonly Random Random = new();

    // Helper methods to generate random latitude and longitude in Tashkent
    public static double GetRandomLatitude()
    {
        return 41.2646 + Random.NextDouble() * (41.3663 - 41.2646);
    }

    public static double GetRandomLongitude()
    {
        return 69.2003 + Random.NextDouble() * (69.3667 - 69.2003);
    }
}

