using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Info
{
    public static Dictionary<string, Vector3> locations = new Dictionary<string, Vector3>()
    {
        { "Madrid", new Vector3(11.2700005f, 14.1899996f, -8.56000042f) },
        { "San Francisco", new Vector3(-14.2299995f,14.0200005f,-2.3499999f) },
        { "Shanghai", new Vector3(3.6400001f,7.21999979f,18.3999996f) },
        { "Paris", new Vector3(11.1300001f,15.6400003f,-5.75f ) },
        { "Dubai", new Vector3(18.3099995f,6.26000023f,5.48000002f) }
    };

    public static List<Tuple<string, string>> stringCityRoutes = new List<Tuple<string, string>>()
    {
        new Tuple<string, string>("Madrid", "Dubai"),
        new Tuple<string, string>("Madrid", "Paris"),
        new Tuple<string, string>("Paris", "San Francisco"),
        new Tuple<string, string>("Dubai", "Shanghai"),
        new Tuple<string, string>("Paris", "Shanghai"),
        new Tuple<string, string>("San Francisco", "Shanghai")
    };

    public static Dictionary<string, string> stringCityCodes = new Dictionary<string, string>()
    {
        { "Madrid", "mad" },
        { "San Francisco", "sfo" },
        { "Shanghai", "pvg" },
        { "Paris", "cdg" },
        { "Dubai", "dxb" }
    };

    public static Dictionary<string, Airport> savedAirports = new Dictionary<string, Airport>();
    public static Dictionary<string, Route> savedRoutes = new Dictionary<string, Route>();
    public static List<Flight> flights = new List<Flight>();
    public static List<Airplane> airplanes = new List<Airplane>();

    public static List<Route> userRoutes = new List<Route>();

    public static Dictionary<Airport, List<Airplane>> airplanesGoingFromEmptyAirport = new Dictionary<Airport, List<Airplane>>();

    public static Dictionary<Airport, List<RouteAssigner.Edge>> DijkstraGraph { get; private set; }

    public static AirportUI airportUI;

    public static Camera playerCamera;


    public static void CalculateDijkstraGraph()
    {
        // Initialize Dijkstra Graph
        if (DijkstraGraph == null)
        {
            DijkstraGraph = new Dictionary<Airport, List<RouteAssigner.Edge>>();

            foreach (Airport airport in savedAirports.Values.ToList())
            {
                DijkstraGraph.Add(airport, new List<RouteAssigner.Edge>());
            }
        }

        // Make sure all airports are included
        foreach (Airport airport in savedAirports.Values)
        {
            if (!DijkstraGraph.ContainsKey(airport))
            {
                DijkstraGraph.Add(airport, new List<RouteAssigner.Edge>());
            }
        }

        // Clear all airport information
        foreach (Airport airport in DijkstraGraph.Keys)
        {
            DijkstraGraph[airport].Clear();
        }

        // Save edges information in Dijkstra Graph
        foreach (Route route in savedRoutes.Values)
        {
            Airport airport1 = route.airport1;
            Airport airport2 = route.airport2;
            double distance = route.distance;

            // Forward: airport1 -> airport2 
            List<RouteAssigner.Edge> edges1 = DijkstraGraph[airport1];

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
            List<RouteAssigner.Edge> edges2 = DijkstraGraph[airport2];

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

    public static Airport GetTakeoffAirportOfAirplane(Airplane airplane)
    {
        Flight flight = GetFlightOfAirplane(airplane);

        if (flight == null)
        {
            return null;
        }

        return flight.airportOrig;
    }

    public static Airport GetAirportOfAirplane(Airplane airplane)
    {
        foreach (Airport airport in savedAirports.Values)
        {
            if (airport.hangar.Contains(airplane))
            {
                return airport;
            }
        }

        return null;
    }

    public static Airport GetLandingAirportOfAirplane(Airplane airplane)
    {
        Airport takeoffAirport = GetTakeoffAirportOfAirplane(airplane);

        foreach (Route route in savedRoutes.Values)
        {
            if (takeoffAirport == route.airport1 || takeoffAirport == route.airport2)
            {
                if (takeoffAirport == route.airport1)
                {
                    return route.airport2;
                }
                else if (takeoffAirport == route.airport2)
                {
                    return route.airport1;
                }
            }
        }

        return null;
    }

    public static Flight GetFlightOfAirplane(Airplane airplane)
    {
        foreach (Flight flight in flights)
        {
            if (flight.airplane == airplane)
            {
                return flight;
            }
        }

        return null;
    }

    public static Route GetRouteOfAirplane(Airplane airplane)
    {
        foreach (Route route in savedRoutes.Values)
        {
            if (route.airplanes.Contains(airplane))
            {
                return route;
            }
        }

        return null;
    }
}
