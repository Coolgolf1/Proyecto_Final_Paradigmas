using System.Collections.Generic;
using System.IO;
using UnityEngine;

static class Auxiliary
{
    private static string _pathName = Path.Combine(Application.streamingAssetsPath, "../flight_distance.csv");

    private static double _defaultDistance = 10000;

    private static InfoSingleton _info = InfoSingleton.GetInstance();

    public static double GetDistanceBetweenAirports(Airport airport1, Airport airport2)
    {
        if (airport1 == airport2)
        {
            return 0;
        }

        string[] lines = File.ReadAllLines(_pathName)[1..];

        string a1 = airport1.Name;
        string a2 = airport2.Name;

        string codeA1 = _info.stringCityCodes[a1];
        string codeA2 = _info.stringCityCodes[a2];

        foreach (string line in lines)
        {
            string[] data = line.Split(",");
            string codes = data[0];

            if (codes == $"{codeA1}-{codeA2}" || codes == $"{codeA2}-{codeA1}")
            {
                double distance = double.Parse(data[1]);
                return distance;
            }
        }

        // If search fails
        Debug.Log($"DISTANCE FAIL SEARCH {airport1}-{airport2}.");
        return _defaultDistance;
    }

    public static void LoadRouteDistances(Dictionary<string, Route> routes)
    {
        string[] lines = File.ReadAllLines(_pathName)[1..];

        foreach (string routeName in routes.Keys)
        {
            Route route = routes[routeName];

            string a1 = route.Airport1.Name;
            string a2 = route.Airport2.Name;

            string codeA1 = _info.stringCityCodes[a1];
            string codeA2 = _info.stringCityCodes[a2];

            bool found = false;
            foreach (string line in lines)
            {
                string[] data = line.Split(",");
                string routeCodes = data[0];

                if (routeCodes == $"{codeA1}-{codeA2}" || routeCodes == $"{codeA2}-{codeA1}")
                {
                    double distance = double.Parse(data[1]);
                    route.SetDistance(distance);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                route.SetDistance(_defaultDistance);

                // REMOVE DEBUG AFTER MAKING SURE THIS WORKS 100%!!! =================================
                Debug.Log($"ERROR LOADING DISTANCE FOR ROUTE: {route.name}.");
            }
        }
    }

    public static void CalculateDijkstraGraph()
    {
        // Make sure all airports are included
        foreach (Airport airport in _info.savedAirports.Values)
        {
            if (!_info.DijkstraGraph.ContainsKey(airport))
            {
                _info.DijkstraGraph.Add(airport, new List<RouteAssigner.Edge>());
            }
        }

        // Clear all airport information
        foreach (Airport airport in _info.DijkstraGraph.Keys)
        {
            _info.DijkstraGraph[airport].Clear();
        }

        // Save edges information in Dijkstra Graph
        foreach (Route route in _info.savedRoutes.Values)
        {
            Airport airport1 = route.Airport1;
            Airport airport2 = route.Airport2;
            double distance = route.Distance;

            // Forward: airport1 -> airport2 
            List<RouteAssigner.Edge> edges1 = _info.DijkstraGraph[airport1];

            // Check if edge from airport1 to airport2 already exists
            bool exists1 = false;
            foreach (RouteAssigner.Edge edge in edges1)
            {
                if (edge.To == airport2)
                {
                    exists1 = true;
                }
            }

            if (!exists1)
            {
                edges1.Add(new RouteAssigner.Edge(airport2, distance));
            }

            // Reverse: airport2 -> airport1
            List<RouteAssigner.Edge> edges2 = _info.DijkstraGraph[airport2];

            // Check if edge from airport2 to airport1 already exists
            bool exists2 = false;
            foreach (RouteAssigner.Edge edge in edges2)
            {
                if (edge.To == airport1)
                {
                    exists2 = true;
                }
            }

            if (!exists2)
            {
                edges2.Add(new RouteAssigner.Edge(airport1, distance));
            }
        }
    }
}
