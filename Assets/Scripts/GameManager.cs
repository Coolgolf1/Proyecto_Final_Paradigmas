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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        new GameObject("Init");
        _info.DistanceAirports = Auxiliary.GetDistancesFromCSV();


        // Define UI and Camera Objects
        _info.airportUI = airportUI;
        _info.flightUI = flightUI;
        _info.playerCamera = playerCamera;

        // Initialise initialliser
        _init = gameObject.AddComponent<Init>();

        // Add listeners
        GameEvents.OnPlayEnter.AddListener(StartGame);
        GameEvents.OnPlayExit.AddListener(ResetGame);
    }

    public void ResetGame()
    {
        _info.ResetAirplanes();
        _info.ResetAirports();
        _info.ResetFlights();
        _info.ResetRoutes();
    }

    public void StartGame()
    {
        // Initialise Factory
        _airplaneFactory.Initialise(airplaneSpawner);

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

        Airplane airplane3 = (Airplane)_airplaneFactory.Build(AirplaneTypes.Large, earth.transform);
        _info.airplanes.Add(airplane3);

        Airplane airplane4 = (Airplane)_airplaneFactory.Build(AirplaneTypes.Large, earth.transform);
        _info.airplanes.Add(airplane4);

        _info.savedAirports["Madrid"].Hangar.Add(airplane);
        _info.savedAirports["Dubai"].Hangar.Add(airplane2);
        _info.savedAirports["Madrid"].Hangar.Add(airplane3);
        _info.savedAirports["Shanghai"].Hangar.Add(airplane4);


        // Init travellers in each airport
        _init.InitTravellersInAirports();

        // Load the real distances from dataset
        Auxiliary.LoadRouteDistances(_info.savedRoutes);

        // Calculate initial Dijkstra Graph
        Auxiliary.CalculateDijkstraGraph();

        List<Airport> destinations = _info.savedAirports.Values.ToList();

        Dictionary<Airport, Dictionary<Airplane, Flight>> airportFlights = new Dictionary<Airport, Dictionary<Airplane, Flight>>();
        foreach (Airport airport in destinations)
        {
            airportFlights[airport] = new Dictionary<Airplane, Flight>();
        }

        // For all travellers in origin airport, assign each of the travellers an airplane
        Queue<Airport> airportQueue = new Queue<Airport>(destinations);

        while (airportQueue.Count > 0)
        {
            Airport origAirport = airportQueue.Dequeue();

            foreach (Airport objAirport in destinations)
            {
                if (origAirport == objAirport)
                {
                    continue;
                }

                if (_info.savedAirports[origAirport.Name].TravellersToAirport[objAirport] <= 0)
                {
                    continue;
                }

                HashSet<Airplane> usedThisIteration = new HashSet<Airplane>();

                while (_info.savedAirports[origAirport.Name].TravellersToAirport[objAirport] > 0)
                {
                    Flight flight;

                    (Airplane objAirplane, Airport nextHop) = _info.savedAirports[origAirport.Name].FindHopForTravellersToAirport(objAirport);

                    if (objAirplane is null || nextHop is null)
                    {
                        break;
                    }

                    if (usedThisIteration.Contains(objAirplane))
                    {
                        break; // no more airplanes left
                    }
                    usedThisIteration.Add(objAirplane);

                    if (airportFlights[objAirport].Keys.Contains(objAirplane))
                    {
                        flight = airportFlights[objAirport][objAirplane];
                    }
                    else
                    {
                        GameObject flightGO = new GameObject();
                        flightGO.name = $"{_info.savedAirports[origAirport.Name].Name}-{nextHop.Name}";
                        flight = flightGO.AddComponent<Flight>();

                        flight.Initialise(_info.savedAirports[origAirport.Name], nextHop, _info.savedRoutes[$"{origAirport.Name}-{nextHop.Name}"], objAirplane);

                        _info.flights.Add(flight);
                        airportFlights[origAirport][objAirplane] = flight;
                    }

                    flight.Embark(_info.savedAirports[origAirport.Name].TravellersToAirport[objAirport], objAirport);
                }
            }
        }

        foreach (Airport airport in destinations)
        {
            foreach (Flight flight in airportFlights[airport].Values)
            {
                flight.StartFlight();
            }
        }

    }

    // Update is called once per frame
    private void Update()
    {
    }
}