using System.Collections.Generic;
using System.IO;
using UnityEngine;

internal static class Auxiliary
{
    private static string _pathName = Path.Combine(Application.streamingAssetsPath, "../flight_distance.csv");

    private static double _defaultDistance = 10000;

    private static InfoSingleton _info = InfoSingleton.GetInstance();

    public static double GetDirectDistanceBetweenAirports(Airport airport1, Airport airport2)
    {

        if (airport1 == airport2)
        {
            return 0;
        }
        else
        {


            string a1 = airport1.Name;
            string a2 = airport2.Name;

            string codeA1 = _info.stringCityCodes[a1];
            string codeA2 = _info.stringCityCodes[a2];

            if (_info.DistanceAirports.ContainsKey($"{codeA1}-{codeA2}"))
            {

                return _info.DistanceAirports[$"{codeA1}-{codeA2}"];
            }
            else if (_info.DistanceAirports.ContainsKey($"{codeA2}-{codeA1}"))
            {
                return _info.DistanceAirports[$"{codeA2}-{codeA1}"];
            }
            else
            {
                Debug.Log("not found");
                return _defaultDistance;
            }
        }


    }

    //public static double GetDistanceBetweenAirports(Airport airport1, Airport airport2)
    //{

    //}

    public static Dictionary<string, double> GetDistancesFromCSV()
    {


        string[] lines = File.ReadAllLines(_pathName)[1..];

        Dictionary<string, double> dictDistances = new Dictionary<string, double>();

        foreach (string line in lines)
        {

            string[] data = line.Split(",");
            string codes = data[0];

            double distance = double.Parse(data[1]);

            dictDistances[codes] = distance;
        }

        return dictDistances;
    }

    public static void LoadRouteDistances(Dictionary<string, Route> routes)
    {
        //string[] lines = File.ReadAllLines(_pathName)[1..];

        foreach (string routeName in routes.Keys)
        {
            Route route = routes[routeName];

            route.SetDistance(GetDirectDistanceBetweenAirports(route.Airport1, route.Airport2));

        }
    }

    public static void CalculateDijkstraGraph()
    {
        // Make sure all airports are included
        foreach (Airport airport in _info.savedAirports.Values)
        {
            if (!DijkstraGraph.graph.ContainsKey(airport))
            {
                DijkstraGraph.graph.Add(airport, new List<Edge>());
            }
        }

        // Clear all airport information
        foreach (Airport airport in DijkstraGraph.graph.Keys)
        {
            DijkstraGraph.graph[airport].Clear();
        }

        // Save edges information in Dijkstra Graph
        foreach (Route route in _info.savedRoutes.Values)
        {
            Airport airport1 = route.Airport1;
            Airport airport2 = route.Airport2;
            double distance = route.Distance;

            // Forward: airport1 -> airport2
            List<Edge> edges1 = DijkstraGraph.graph[airport1];

            // Check if edge from airport1 to airport2 already exists
            bool exists1 = false;
            foreach (Edge edge in edges1)
            {
                if (edge.To == airport2)
                {
                    exists1 = true;
                }
            }

            if (!exists1)
            {
                edges1.Add(new Edge(airport2, distance));
            }

            // Reverse: airport2 -> airport1
            List<Edge> edges2 = DijkstraGraph.graph[airport2];

            // Check if edge from airport2 to airport1 already exists
            bool exists2 = false;
            foreach (Edge edge in edges2)
            {
                if (edge.To == airport1)
                {
                    exists2 = true;
                }
            }

            if (!exists2)
            {
                edges2.Add(new Edge(airport1, distance));
            }
        }
    }

    public static void InitTravellersInAirports()
    {
        foreach (Airport airport in _info.savedAirports.Values)
        {
            airport.InitTravellers();
        }
    }
}