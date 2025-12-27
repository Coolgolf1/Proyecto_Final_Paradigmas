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
        { "Lima",  new Vector3(-9.59000015f,0.0700000003f,-17.8500004f)},
        { "Buenos Aires", new Vector3(-3.50999999f,-8.25f,-18.0499992f) },
        { "Mexico City",  new Vector3(-14.6000004f,9.71000004f,-10.25f)},
        { "Caracas",  new Vector3(-6.30000019f,7.28999996f,-17.9099998f)},
        { "Winnipeg",  new Vector3(-9.39999962f,17.0599995f,-5.78999996f)},
        { "Miami",  new Vector3(-9.18000031f,11.8900003f,-13.54f)},
        { "Asuncion",  new Vector3(-3.20000005f,-4.76999998f,-19.5499992f)},
        { "Tokyo",  new Vector3(-1.76900005f,8.82199955f,18.0709991f)},
        { "Bangkok",  new Vector3(10.3549995f,1.41400003f,17.4120007f)},
        { "Port Moresby",  new Vector3(-5.46099997f,-6.25299978f,18.6259995f)},
        { "Manila",  new Vector3(3.7579999f,1.52999997f,19.9610004f)},
        { "Singapore",  new Vector3(9.40400028f,-3.04994011f,17.6700001f)},
        { "Yakutsk",  new Vector3(1.02100003f,15.9460001f,12.4589996f)},
        { "Antananarivo",  new Vector3(18.7779999f,-7.82600021f,0.606000006f)},
        { "Nairobi",  new Vector3(20.2029991f,-0.897000015f,-1.64699996f)},
        { "Dakar",  new Vector3(10.7150002f,7.61399984f,-15.6230001f)}

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
        new Tuple<string, string>("Paris", "Oslo"),

        // Nuevas rutas para cubrir todos los aeropuertos
        new Tuple<string, string>("Buenos Aires", "Sao Paulo"),
        new Tuple<string, string>("Mexico City", "Miami"),
        new Tuple<string, string>("Caracas", "Miami"),
        new Tuple<string, string>("Winnipeg", "New York"),
        new Tuple<string, string>("Miami", "New York"),
        new Tuple<string, string>("Asuncion", "Buenos Aires"),
        new Tuple<string, string>("Tokyo", "Shanghai"),
        new Tuple<string, string>("Bangkok", "Singapore"),
        new Tuple<string, string>("Port Moresby", "Sydney"),
        new Tuple<string, string>("Manila", "Singapore"),
        new Tuple<string, string>("Singapore", "Dubai"),
        new Tuple<string, string>("Yakutsk", "Moscow"),
        new Tuple<string, string>("Antananarivo", "Nairobi"),
        new Tuple<string, string>("Nairobi", "Cairo"),
        new Tuple<string, string>("Dakar", "Madrid"),
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
        { "Lima", "lim" },
        { "Buenos Aires", "aep" },
        { "Mexico City", "mex" },
        { "Caracas", "ccs" },
        { "Winnipeg", "ymg" },
        { "Miami", "mia" },
        { "Asuncion", "asu" },
        { "Tokyo", "hnd" },
        { "Bangkok", "bkk" },
        { "Port Moresby", "pom" },
        { "Manila", "mnl" },
        { "Singapore", "sin" },
        { "Yakutsk", "yks" },
        { "Antananarivo", "tnr" },
        { "Nairobi", "nbo" },
        { "Dakar", "dss" },
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
    public GameObject earth;

    public Airplane airplaneInHangar;

    public NotificationSystem notificationSystem;


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
        Player.UnlockedAirports.Clear();
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

    public List<Airplane> GetAirplanesInRoute(Route route)
    {
        List<Airplane> airplanesInRoute = new List<Airplane>();
        foreach (Flight flight in flights)
        {
            if (flight.Route == route)
            {
                airplanesInRoute.Add(flight.Airplane);
            }
        }

        return airplanesInRoute;
    }

    public void EnablePriority(Airport airport)
    {
        foreach (Airport airportItem in savedAirports.Values)
        {
            if (airportItem == airport)
            {
                airportItem.PriorityOn = true;
            }
            else
            {
                airportItem.PriorityOn = false;
            }
        }
    }

    public void GoToHangar(Airplane airplane)
    {
        Time.timeScale = 0;
        airplaneInHangar = airplane;
        SceneManager.LoadScene("Hangar", LoadSceneMode.Additive);
    }

    public void GoToSpace()
    {
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("Hangar");
    }

    public void ToggleMute()
    {
        if (playerCamera.GetComponent<PlayerMovement>() is SpaceCamera camera)
        {
            AudioSource source = camera.GetComponent<AudioSource>();
            source.mute = !source.mute;
        }
    }
}