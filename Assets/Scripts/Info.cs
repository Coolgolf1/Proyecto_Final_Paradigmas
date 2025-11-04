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

    public static List<Tuple<string, string>> availableRoutes = new List<Tuple<string, string>>()
    {
        new Tuple<string, string>("Madrid", "Dubai"),
        new Tuple<string, string>("Madrid", "Paris"),
        new Tuple<string, string>("Paris", "San Francisco"),
        new Tuple<string, string>("Dubai", "Shanghai"),
        new Tuple<string, string>("Paris", "Shanghai"),
        new Tuple<string, string>("San Francisco", "Shanghai")
    };

    public static Dictionary<string, string> codes = new Dictionary<string, string>()
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

    public static Airport GetTakeoffAirportOfAirplane(Airplane airplane)
    {
        // FIX THIS TO USE THINGS WE HAVE NOT STATIC CLASS
        foreach (Airport airport in AirportList.items)
        {
            if (airport.hangar.Contains(airplane))
            {
                return airport;
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
        foreach (Route route in savedRoutes.Values.ToList())
        {
            if (route.airplanes.Contains(airplane))
            {
                return route;
            }
        }

        return null;
    }
}
