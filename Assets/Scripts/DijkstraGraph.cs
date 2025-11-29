using System.Collections.Generic;

public static class DijkstraGraph
{
    public static Dictionary<Airport, List<RouteAssigner.Edge>> graph { get; private set; }

    private static InfoSingleton _info = InfoSingleton.GetInstance();

    public static void Initialise()
    {
        graph = new Dictionary<Airport, List<RouteAssigner.Edge>>();

        foreach (Airport airport in _info.savedAirports.Values)
        {
            graph.Add(airport, new List<RouteAssigner.Edge>());
        }
    }
}
