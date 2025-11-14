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
    private Init _init;
    private GameState _currentState = new PlayState();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        new GameObject("Init");

        // Define UI and Camera Objects
        _info.airportUI = airportUI;
        _info.flightUI = flightUI;
        _info.playerCamera = playerCamera;

        // Initialise Factory
        _airplaneFactory.Initialise(airplaneSpawner);

        // Initialise initialliser
        _init = gameObject.AddComponent<Init>();

        // Save data of airports 
        _init.SaveDataOfAirports(airportPrefab, earth.transform);

        // Initialise list of empty airports once airports have loaded
        _info.InitEmptyAirportList();

        // Save data of routes
        _init.SaveDataOfRoutes(routePrefab, earth.transform);

        // Create Airplanes with Factory
        Airplane airplane = (Airplane)_airplaneFactory.Build(AirplaneTypes.Large, earth.transform);
        _info.airplanes.Add(airplane);

        Airplane airplane2 = (Airplane)_airplaneFactory.Build(AirplaneTypes.Large, earth.transform);
        _info.airplanes.Add(airplane2);

        _info.savedAirports["Madrid"].Hangar.Add(airplane);
        _info.savedAirports["Madrid"].Hangar.Add(airplane2);


        // Init travellers in each airport
        _init.InitTravellersInAirports();

        //info.savedAirports["Madrid"].TravellersToAirport[info.savedAirports["Paris"]] = 10;
        //_info.savedAirports["Madrid"].TravellersToAirport[_info.savedAirports["Dubai"]] = 80;
        //_info.savedAirports["Madrid"].TravellersToAirport[_info.savedAirports["San Francisco"]] = 10;
        //_info.savedAirports["Dubai"].TravellersToAirport[_info.savedAirports["Paris"]] = 50;

        //info.savedAirports["Madrid"].TravellersToAirport[info.savedAirports["Paris"]] = 10;

        // Load the real distances from dataset
        Auxiliary.LoadRouteDistances(_info.savedRoutes);

        // Calculate initial Dijkstra Graph
        Auxiliary.CalculateDijkstraGraph();

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

        Debug.Log("FINISH INIT");
    }

    // Update is called once per frame
    private void Update()
    {
    }
}