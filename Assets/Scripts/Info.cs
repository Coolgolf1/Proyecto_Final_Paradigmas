using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfoSingleton
{
    public Dictionary<string, Vector3> locations = new Dictionary<string, Vector3>()
    {
        { "Madrid", new Vector3(11.2700005f, 14.1899996f, -8.56000042f) },
        { "San Francisco", new Vector3(-14.2299995f,14.0200005f,-2.3499999f) },
        { "Shanghai", new Vector3(3.6400001f,7.21999979f,18.3999996f) },
        { "Paris", new Vector3(11.1300001f,15.6400003f,-5.75f ) },
        { "Dubai", new Vector3(18.3099995f,6.26000023f,5.48000002f) }
    };

    public List<Tuple<string, string>> stringCityRoutes = new List<Tuple<string, string>>()
    {
        new Tuple<string, string>("Madrid", "Dubai"),
        new Tuple<string, string>("Madrid", "Paris"),
        new Tuple<string, string>("Paris", "San Francisco"),
        new Tuple<string, string>("Dubai", "Shanghai"),
        new Tuple<string, string>("Paris", "Shanghai"),
        new Tuple<string, string>("San Francisco", "Shanghai")
    };

    public Dictionary<string, string> stringCityCodes = new Dictionary<string, string>()
    {
        { "Madrid", "mad" },
        { "San Francisco", "sfo" },
        { "Shanghai", "pvg" },
        { "Paris", "cdg" },
        { "Dubai", "dxb" }
    };

    // Saved data
    public Dictionary<string, Airport> savedAirports = new Dictionary<string, Airport>();

    public Dictionary<string, Route> savedRoutes = new Dictionary<string, Route>();
    public List<Flight> flights = new List<Flight>();
    public List<Airplane> airplanes = new List<Airplane>();
    public Dictionary<Airport, List<Airplane>> airplanesGoingFromEmptyAirport = new Dictionary<Airport, List<Airplane>>();

    // Game information
    public int totalTravellersReceived { get; set; } = 0;

    // Dijkstra graph
    public Dictionary<Airport, List<RouteAssigner.Edge>> DijkstraGraph { get; private set; }

    // GameObjects set in GameManager
    public AirportUI airportUI;

    public FlightUI flightUI;
    public Camera playerCamera;

    // Singleton
    private static InfoSingleton _instance;

    private InfoSingleton()
    {
        DijkstraGraph = new Dictionary<Airport, List<RouteAssigner.Edge>>();

        foreach (Airport airport in savedAirports.Values.ToList())
        {
            DijkstraGraph.Add(airport, new List<RouteAssigner.Edge>());
        }
    }

    public static InfoSingleton GetInstance()
    {
        if (_instance == null)
        {
            _instance = new InfoSingleton();
        }
        return _instance;
    }

    public void InitEmptyAirportList()
    {
        foreach (Airport airport in savedAirports.Values)
        {
            airplanesGoingFromEmptyAirport[airport] = new List<Airplane>();
        }
    }

    public Airport GetTakeoffAirportOfAirplane(Airplane airplane)
    {
        Flight flight = GetFlightOfAirplane(airplane);

        if (flight == null)
        {
            return null;
        }

        return flight.AirportOrig;
    }

    public Airport GetAirportOfAirplane(Airplane airplane)
    {
        foreach (Airport airport in savedAirports.Values)
        {
            if (airport.Hangar.Contains(airplane))
            {
                return airport;
            }
        }

        return null;
    }

    public Airport GetLandingAirportOfAirplane(Airplane airplane)
    {
        Airport takeoffAirport = GetTakeoffAirportOfAirplane(airplane);

        foreach (Route route in savedRoutes.Values)
        {
            if (takeoffAirport == route.Airport1 || takeoffAirport == route.Airport2)
            {
                if (takeoffAirport == route.Airport1)
                {
                    return route.Airport2;
                }
                else if (takeoffAirport == route.Airport2)
                {
                    return route.Airport1;
                }
            }
        }

        return null;
    }

    public Flight GetFlightOfAirplane(Airplane airplane)
    {
        foreach (Flight flight in flights)
        {
            if (flight.Airplane == airplane)
            {
                return flight;
            }
        }

        return null;
    }
}