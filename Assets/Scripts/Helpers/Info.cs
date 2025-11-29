using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InfoSingleton
{
    public Dictionary<string, Vector3> locations = new Dictionary<string, Vector3>()
    {
        { "Madrid", new Vector3(11.2700005f, 14.1899996f, -8.56000042f) },
        { "San Francisco", new Vector3(-14.2299995f,14.0200005f,-2.3499999f) },
        { "Shanghai", new Vector3(3.6400001f,7.21999979f,18.3999996f) },
        { "Paris", new Vector3(11.1300001f,15.6400003f,-5.75f ) },
        { "Dubai", new Vector3(18.3099995f,6.26000023f,5.48000002f) },
        { "Sao Paulo", new Vector3(-0.600000024f,-4.26000023f,-19.7099991f) },
        { "Sydney", new Vector3(-5.73000002f,-12.9899998f,14.3000002f)},
        { "New York", new Vector3(-6.86000013f,15.25f,-11.3559999f) },
        { "Cape Town", new Vector3(15.2600002f,-10.5799999f,-8.17000008f) },
        { "Honolulu", new Vector3(-16.9699993f,6.92999983f,8.11999989f) },
        { "Mumbai", new Vector3(16.3600006f,3.73000002f,11.1999998f) },
        { "Moscow", new Vector3(12.3699999f,15.8000002f,1.87f) },
        { "Oslo", new Vector3(9.23999977f,17.7600002f,-2.05999994f) },
        { "Cairo", new Vector3(17.9599991f,9.22999954f,-1.21000004f) },
        { "Lima",  new Vector3(-9.59000015f,0.0700000003f,-17.8500004f)}
    };

    public List<Tuple<string, string>> stringCityRoutes = new List<Tuple<string, string>>()
    {
        new Tuple<string, string>("Madrid", "Dubai"),
        new Tuple<string, string>("Madrid", "Paris"),
        new Tuple<string, string>("Dubai", "Shanghai"),
        new Tuple<string, string>("San Francisco", "Shanghai"),
        new Tuple<string, string>("Madrid", "New York"),
        new Tuple<string, string>("New York", "San Francisco"),
        new Tuple<string, string>("New York", "Sao Paulo"),
        new Tuple<string, string>("San Francisco", "Honolulu"),
        new Tuple<string, string>("Lima", "Sao Paulo"),
        new Tuple<string, string>("Lima", "Honolulu"),
        new Tuple<string, string>("Mumbai", "Dubai"),
        new Tuple<string, string>("Oslo", "Moscow"),
        new Tuple<string, string>("Moscow", "Dubai"),
        new Tuple<string, string>("Sydney", "Mumbai"),
        new Tuple<string, string>("Sydney", "Shanghai"),
        new Tuple<string, string>("Shanghai", "Honolulu"),
        new Tuple<string, string>("Cape Town", "Cairo"),
        new Tuple<string, string>("Cape Town", "Sao Paulo"),
        new Tuple<string, string>("Dubai", "Cairo"),
        new Tuple<string, string>("Oslo", "Cairo"),
        new Tuple<string, string>("Oslo", "New York"),
        new Tuple<string, string>("Madrid", "Sao Paulo"),
        new Tuple<string, string>("Paris", "Oslo")
    };

    public Dictionary<string, string> stringCityCodes = new Dictionary<string, string>()
    {
        { "Madrid", "mad" },
        { "San Francisco", "sfo" },
        { "Shanghai", "pvg" },
        { "Paris", "cdg" },
        { "Dubai", "dxb" },
        { "Sao Paulo", "gru" },
        { "Sydney", "syd" },
        { "New York", "jfk" },
        { "Cape Town", "cpt" },
        { "Honolulu", "hnl" },
        { "Mumbai", "bom" },
        { "Moscow", "svo" },
        { "Oslo", "osl" },
        { "Cairo", "cai" },
        { "Lima", "lim" }
    };

    // Saved data
    public Dictionary<string, Airport> savedAirports = new Dictionary<string, Airport>();
    public Dictionary<string, double> DistanceAirports;

    public Dictionary<string, Route> savedRoutes = new Dictionary<string, Route>();
    public List<Flight> flights = new List<Flight>();
    public List<Airplane> airplanes = new List<Airplane>();
    public Dictionary<Airport, List<Airplane>> airplanesGoingFromEmptyAirport = new Dictionary<Airport, List<Airplane>>();

    // Game information
    public int totalTravellersReceived { get; set; } = 0;

    // GameObjects set in GameManager
    public AirportUI airportUI;
    public FlightUI flightUI;
    public Camera playerCamera;

    public Airplane airplaneToHangar;

    // Singleton
    private static InfoSingleton _instance;

    public static InfoSingleton GetInstance()
    {
        if (_instance == null)
        {
            _instance = new InfoSingleton();
        }
        return _instance;
    }

    public void ResetAirports()
    {
        foreach (Airport airport in savedAirports.Values)
        {
            GameObject.Destroy(airport.gameObject);
        }

        savedAirports.Clear();
    }

    public void ResetRoutes()
    {
        foreach (Route route in savedRoutes.Values)
        {
            GameObject.Destroy(route.gameObject);
        }

        savedRoutes.Clear();
    }

    public void ResetFlights()
    {
        foreach (Flight flight in flights)
        {
            GameObject.Destroy(flight.gameObject);
        }

        flights.Clear();
    }

    public void ResetAirplanes()
    {
        foreach (Airplane airplane in airplanes)
        {
            GameObject.Destroy(airplane.gameObject);
        }

        airplanes.Clear();
        airplanesGoingFromEmptyAirport.Clear();
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

    public List<Route> GetRoutesOfAirport(Airport airport)
    {
        List<Route> routes = new List<Route>();
        foreach (Route route in savedRoutes.Values)
        {
            if (airport == route.Airport1 || airport == route.Airport2)
            {
                routes.Add(route);
            }
        }

        return routes;
    }

    public Route GetRouteOfAirplane(Airplane airplane)
    {
        Flight flight = GetFlightOfAirplane(airplane);
        if (flight is not null)
        {
            return flight.Route;
        }
        return null;
    }

    public void GoToHangar(Airplane airplane)
    {
        Time.timeScale = 0;
        airplaneToHangar = airplane;
        SceneManager.LoadScene("Hangar", LoadSceneMode.Additive);
    }

    public void GoToSpace()
    {
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("Hangar");
    }
}