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

    private InfoSingleton _info = InfoSingleton.GetInstance();
    private AirplaneFactory _airplaneFactory = AirplaneFactory.GetInstance();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _info.airportUI = airportUI;
        _info.flightUI = flightUI;
        _info.playerCamera = playerCamera;

        // Initialize Factory
        _airplaneFactory.Initialise(airplaneSpawner);


        // Save data of airports 
        foreach (string city in _info.locations.Keys)
        {
            // Create Airport GameObject
            GameObject airportGO = Instantiate(airportPrefab, earth.transform);
            airportGO.name = city;

            // Get Components of GameObject
            Airport airport = airportGO.GetComponent<Airport>();
            Location location = airportGO.GetComponentInChildren<Location>();

            // Save location and airport properties
            location.Initialise(id: city, name: $"{city}_Loc", coords: _info.locations[city]);
            airport.Initialise(id: _info.stringCityCodes[city], name: city, location: location);

            // Save airport 
            _info.savedAirports[city] = airport;
        }

        // Create Airplanes with Factory
        Airplane airplane = (Airplane)_airplaneFactory.Build(AirplaneTypes.Large, earth.transform);
        _info.airplanes.Add(airplane);

        Airplane airplane2 = (Airplane)_airplaneFactory.Build(AirplaneTypes.Large, earth.transform);
        _info.airplanes.Add(airplane2);

        // Save data of routes
        foreach (Tuple<string, string> routeTuple in _info.stringCityRoutes)
        {
            GameObject routeGO = Instantiate(routePrefab, earth.transform);
            routeGO.name = $"{routeTuple.Item1}-{routeTuple.Item2}";
            Route route = routeGO.GetComponent<Route>();
            route.Initialise(airport1: _info.savedAirports[routeTuple.Item1], airport2: _info.savedAirports[routeTuple.Item2]);
            _info.savedRoutes[routeGO.name] = route;
            _info.savedRoutes[$"{routeTuple.Item2}-{routeTuple.Item1}"] = route;
        }

        _info.savedAirports["Madrid"].Hangar.Add(airplane);
        _info.savedAirports["Madrid"].Hangar.Add(airplane2);


        // Init travellers in each airport
        foreach (Airport airport in _info.savedAirports.Values)
        {
            airport.InitTravellers();

            _info.airplanesGoingFromEmptyAirport[airport] = new List<Airplane>();
        }

        //info.savedAirports["Madrid"].TravellersToAirport[info.savedAirports["Paris"]] = 10;
        _info.savedAirports["Madrid"].TravellersToAirport[_info.savedAirports["Dubai"]] = 80;
        _info.savedAirports["Madrid"].TravellersToAirport[_info.savedAirports["San Francisco"]] = 10;
        _info.savedAirports["Dubai"].TravellersToAirport[_info.savedAirports["Paris"]] = 50;

        //info.savedAirports["Madrid"].TravellersToAirport[info.savedAirports["Paris"]] = 10;

        // Load the real distances from dataset
        Auxiliary.LoadRouteDistances(_info.savedRoutes);

        // Calculate initial Dijkstra Graph
        _info.CalculateDijkstraGraph();

        // Create Madrid-Dubai Flight
        //Flight flight = Auxiliary.CreateFlight(info.savedAirports["Madrid"], info.savedAirports["Dubai"], info.savedRoutes["Madrid-Dubai"], airplane);

        List<Airport> destinations = _info.savedAirports.Values.ToList();

        Dictionary<Airplane, Flight> madridFlights = new Dictionary<Airplane, Flight>();

        // For all travellers in origin airport, assign each of the travellers an airplane

        Queue<Airport> airportQueue = new Queue<Airport>(destinations);

        while (airportQueue.Count > 0)
        {
            Airport airport = airportQueue.Dequeue();

            if (airport == _info.savedAirports["Madrid"])
            {
                continue;
            }

            if (_info.savedAirports["Madrid"].TravellersToAirport[airport] <= 0)
            {
                continue;
            }

            HashSet<Airplane> usedThisIteration = new HashSet<Airplane>();

            while (_info.savedAirports["Madrid"].TravellersToAirport[airport] > 0)
            {
                Flight flight;

                (Airplane objAirplane, Airport nextHop) = _info.savedAirports["Madrid"].FindHopForTravellersToAirport(airport);

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
                    flightGO.name = $"{_info.savedAirports["Madrid"].Name}-{nextHop.Name}";
                    flight = flightGO.AddComponent<Flight>();


                    flight.Initialise(_info.savedAirports["Madrid"], nextHop, _info.savedRoutes[$"Madrid-{nextHop.Name}"], objAirplane);

                    _info.flights.Add(flight);
                    madridFlights[objAirplane] = flight;
                }

                flight.Embark(_info.savedAirports["Madrid"].TravellersToAirport[airport], airport);
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