using System.Collections.Generic;

public static class AirportList
{
    public static List<Airport> items { get; private set; }

    public static void Add(Airport airport)
    {
        items.Add(airport);
    }
}