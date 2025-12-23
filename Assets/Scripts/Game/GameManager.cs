using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject airportPrefab;

    

    [SerializeField] private GameObject earth;

    [SerializeField] private AirportUI airportUI;
    [SerializeField] private FlightUI flightUI;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AirplaneSpawner airplaneSpawner;
    [SerializeField] private GameObject routePrefab;
    

    private InfoSingleton _info = InfoSingleton.GetInstance();
    private AirplaneFactory _airplaneFactory = AirplaneFactory.GetInstance();
    private Init _init;
    private bool _mainMenuGame = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        new GameObject("Init");
        _info.DistanceAirports = Auxiliary.GetDistancesFromCSV();
        _info.earth = earth;

        // Define UI and Camera Objects
        _info.airportUI = airportUI;
        _info.flightUI = flightUI;
        _info.playerCamera = playerCamera;

        // Initialise initialliser
        _init = gameObject.AddComponent<Init>();

        // Start game or main menu game
        GameEvents.OnMainMenuEnter.AddListener(StartMainMenuGame);
        GameEvents.OnMainMenuExit.AddListener(StopMainMenuGame);

        // Start game if game or main menu game
        GameEvents.OnPlayEnter.AddListener(StartGame);
        GameEvents.OnMainMenuEnter.AddListener(StartGame);

        // Reset game if end or exit main menu game
        GameEvents.OnPlayExit.AddListener(ResetGame);
        GameEvents.OnMainMenuExit.AddListener(ResetGame);
    }

    public void StartMainMenuGame()
    {
        _mainMenuGame = true;
    }

    public void StopMainMenuGame()
    {
        _mainMenuGame = false;
    }

    public void ResetGame()
    {
        _info.ResetAirplanes();
        _info.ResetAirports();
        _info.ResetFlights();
        _info.ResetRoutes();
        Player.Money = 0;
    }

    public void InitMainMenuAirplanes(Transform earthTransform)
    {
        System.Random rand = new System.Random();

        for (int i = 0; i < 20; i++)
        {
            int r = rand.Next(3);
            Airplane airplane = (Airplane)_airplaneFactory.Build(AirplaneTypes.Small + r, earthTransform);

            int a = rand.Next(_info.savedAirports.Count);
            string key = _info.savedAirports.Keys.ToList()[a];
            _info.savedAirports[key].Hangar.Add(airplane);
            _info.airplanes.Add(airplane);
        }
    }

    public void StartGame()
    {
        // Initialise Factory
        _airplaneFactory.Initialise(airplaneSpawner);

        // Save data of airports
        _init.SaveDataOfAirports(airportPrefab, earth.transform);

        Player.UnlockedAirports = _info.savedAirports.Values.ToList();

        // Initialise list of empty airports once airports have loaded
        _info.InitEmptyAirportList();

        // Save data of routes

        

        if (_mainMenuGame)
        {
            InitMainMenuAirplanes(earth.transform);
            _init.SaveDataOfRoutes(routePrefab, earth.transform);
        }
        else
        {
            _info.ResetRoutes();
            //// Create Airplanes with Factory
            //Airplane airplane = (Airplane)_airplaneFactory.Build(AirplaneTypes.Large, earth.transform);
            //_info.airplanes.Add(airplane);

            //Airplane airplane2 = (Airplane)_airplaneFactory.Build(AirplaneTypes.Large, earth.transform);
            //_info.airplanes.Add(airplane2);

            //Airplane airplane3 = (Airplane)_airplaneFactory.Build(AirplaneTypes.Large, earth.transform);
            //_info.airplanes.Add(airplane3);

            //Airplane airplane4 = (Airplane)_airplaneFactory.Build(AirplaneTypes.Large, earth.transform);
            //_info.airplanes.Add(airplane4);

            //_info.savedAirports["Madrid"].Hangar.Add(airplane);
            //_info.savedAirports["Dubai"].Hangar.Add(airplane2);
            //_info.savedAirports["Madrid"].Hangar.Add(airplane3);
            //_info.savedAirports["Shanghai"].Hangar.Add(airplane4);

        }

        // Init travellers in each airport
        Auxiliary.InitTravellersInAirports();

        _info.savedAirports["Sao Paulo"].TravellersToAirport[_info.savedAirports["Lima"]] = 500;

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

        List<Airplane> airplaneInFlight = new List<Airplane>();

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
                    else if (airplaneInFlight.Contains(objAirplane))
                        continue;
                    else
                    {
                        GameObject flightGO = new GameObject();
                        flightGO.name = $"{_info.savedAirports[origAirport.Name].Name}-{nextHop.Name}";
                        flight = flightGO.AddComponent<Flight>();

                        flight.Initialise(_info.savedAirports[origAirport.Name], nextHop, _info.savedRoutes[$"{origAirport.Name}-{nextHop.Name}"], objAirplane);

                        _info.flights.Add(flight);
                        airportFlights[origAirport][objAirplane] = flight;

                        airplaneInFlight.Add(objAirplane);
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