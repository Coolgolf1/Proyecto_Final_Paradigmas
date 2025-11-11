using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject airportPrefab;

    [SerializeField] private GameObject routePrefab;

    [SerializeField] private GameObject earth;

    [SerializeField] private AirportUI airportUI;
    [SerializeField] private FlightUI flightUI;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AirplaneSpawner airplaneSpawner;

    private InfoSingleton info = InfoSingleton.GetInstance();
    private AirplaneFactory airplaneFactory = AirplaneFactory.GetInstance();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        info.airportUI = airportUI;
        info.flightUI = flightUI;
        info.playerCamera = playerCamera;

        // Initialize Factory
        airplaneFactory.Initialise(airplaneSpawner);


        // Save data of airports 
        foreach (string city in info.locations.Keys)
        {
            // Create Airport GameObject
            GameObject airportGO = Instantiate(airportPrefab, earth.transform);
            airportGO.name = city;

            // Get Components of GameObject
            Airport airport = airportGO.GetComponent<Airport>();
            Location location = airportGO.GetComponentInChildren<Location>();

            // Save location and airport properties
            location.Initialise(id: city, name: $"{city}_Loc", coords: info.locations[city]);
            airport.Initialise(id: info.stringCityCodes[city], name: city, location: location);

            // Save airport 
            info.savedAirports[city] = airport;
        }

        // Create Airplanes with Factory
        Airplane airplane = (Airplane)airplaneFactory.Build(AirplaneTypes.Large, earth.transform);
        info.airplanes.Add(airplane);

        Airplane airplane2 = (Airplane)airplaneFactory.Build(AirplaneTypes.Large, earth.transform);
        info.airplanes.Add(airplane2);

        // Save data of routes
        foreach (Tuple<string, string> routeTuple in info.stringCityRoutes)
        {
            GameObject routeGO = Instantiate(routePrefab, earth.transform);
            routeGO.name = $"{routeTuple.Item1}-{routeTuple.Item2}";
            Route route = routeGO.GetComponent<Route>();
            route.Initialise(airport1: info.savedAirports[routeTuple.Item1], airport2: info.savedAirports[routeTuple.Item2]);
            info.savedRoutes[routeGO.name] = route;
            info.savedRoutes[$"{routeTuple.Item2}-{routeTuple.Item1}"] = route;
        }

        info.savedAirports["Madrid"].Hangar.Add(airplane);
        info.savedAirports["Madrid"].Hangar.Add(airplane2);


        // Init travellers in each airport
        foreach (Airport airport in info.savedAirports.Values)
        {
            airport.InitTravellers();

            info.airplanesGoingFromEmptyAirport[airport] = new List<Airplane>();
        }

        //info.savedAirports["Madrid"].TravellersToAirport[info.savedAirports["Paris"]] = 10;
        info.savedAirports["Madrid"].TravellersToAirport[info.savedAirports["Dubai"]] = 80;
        info.savedAirports["Madrid"].TravellersToAirport[info.savedAirports["San Francisco"]] = 10;
        info.savedAirports["Dubai"].TravellersToAirport[info.savedAirports["Paris"]] = 50;

        //info.savedAirports["Madrid"].TravellersToAirport[info.savedAirports["Paris"]] = 10;

        // Load the real distances from dataset
        Auxiliary.LoadRouteDistances(info.savedRoutes);

        // Calculate initial Dijkstra Graph
        info.CalculateDijkstraGraph();

        // Create Madrid-Dubai Flight
        //Flight flight = Auxiliary.CreateFlight(info.savedAirports["Madrid"], info.savedAirports["Dubai"], info.savedRoutes["Madrid-Dubai"], airplane);

        List<Airport> destinations = info.savedAirports.Values.ToList();

        Dictionary<Airplane, Flight> madridFlights = new Dictionary<Airplane, Flight>();

        // For all travellers in origin airport, assign each of the travellers an airplane

        Queue<Airport> airportQueue = new Queue<Airport>(destinations);

        while (airportQueue.Count > 0)
        {
            Airport airport = airportQueue.Dequeue();

            if (airport == info.savedAirports["Madrid"])
            {
                continue;
            }

            if (info.savedAirports["Madrid"].TravellersToAirport[airport] <= 0)
            {
                continue;
            }

            HashSet<Airplane> usedThisIteration = new HashSet<Airplane>();

            while (info.savedAirports["Madrid"].TravellersToAirport[airport] > 0)
            {
                Flight flight;

                (Airplane objAirplane, Airport nextHop) = info.savedAirports["Madrid"].FindHopForTravellersToAirport(airport);

                if (objAirplane is null || nextHop is null)
                {
                    break;
                }

                if (usedThisIteration.Contains(objAirplane))
                {
                    break; // no more airplanes left
                }
                usedThisIteration.Add(objAirplane);


                if (madridFlights.Keys.Contains(objAirplane))
                {
                    flight = madridFlights[objAirplane];
                }
                else
                {
                    GameObject flightGO = new GameObject();
                    flightGO.name = $"{info.savedAirports["Madrid"].Name}-{nextHop.Name}";
                    flight = flightGO.AddComponent<Flight>();


                    flight.Initialise(info.savedAirports["Madrid"], nextHop, info.savedRoutes[$"Madrid-{nextHop.Name}"], objAirplane);

                    info.flights.Add(flight);
                    madridFlights[objAirplane] = flight;
                }

                flight.Embark(info.savedAirports["Madrid"].TravellersToAirport[airport], airport);
            }

        }


        foreach (Flight flight in madridFlights.Values)
        {
            flight.StartFlight();
        }


    }

    // Update is called once per frame
    private void Update()
    {
    }
}