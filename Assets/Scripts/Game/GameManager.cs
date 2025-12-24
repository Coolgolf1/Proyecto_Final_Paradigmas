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
    private EconomyManager _economy = EconomyManager.GetInstance();
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
        _economy.mainMenuGame = true;
    }

    public void StopMainMenuGame()
    {
        _mainMenuGame = false;
        _economy.mainMenuGame = false;
    }

    public void ResetGame()
    {
        _info.ResetAirplanes();
        _info.ResetAirports();
        _info.ResetFlights();
        _info.ResetRoutes();
        Player.Money = 0;
        Player.Score = 0;

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
            _info.savedAirports[key].SetCapacity(1000000000);
            _info.airplanes.Add(airplane);
        }
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
        if (_mainMenuGame)
        {
            InitMainMenuAirplanes(earth.transform);

            _init.SaveDataOfRoutes(routePrefab, earth.transform);
            foreach (Airport airport in _info.savedAirports.Values)
            {
                Player.UnlockAirport(airport);
            }

            // Init travellers in each airport
            Auxiliary.InitTravellersInAirports(mainMenuGame: true);
        }
        else
        {
            _info.ResetRoutes();

            System.Random _rand = new System.Random();

            // 1. Get all airports ONCE to avoid re-converting repeatedly
            List<Airport> allAirports = _info.savedAirports.Values.ToList();

            // 2. Filter: Find ONLY airports that actually have neighbors in range
            // We create a list of 'candidates' so we know our random pick will succeed.
            List<Airport> validStarts = new List<Airport>();

            foreach (var airport in allAirports)
            {
                // Check if this airport has at least one reachable neighbor
                if (airport.GetReachableAirportsForRange(GameConstants.smallRange).Count > 0)
                {
                    validStarts.Add(airport);
                }
            }

            // 3. SAFETY CHECK: What if NO airports work?
            if (validStarts.Count == 0)
            {
                Debug.LogError("Infinite Loop Prevented: No airports have neighbors within Small Range!");
                // Handle this case: Maybe increase range, or unlock a default pair?
                return;
            }

            // 4. Now we can safely pick a random one from the VALID list
            Airport initialAirport1 = validStarts[_rand.Next(0, validStarts.Count)];

            // We know this list is not empty because we just checked it in step 2
            List<Airport> reachableAirports = initialAirport1.GetReachableAirportsForRange(GameConstants.smallRange);

            // 5. Pick the second airport
            Airport initialAirport2 = reachableAirports[_rand.Next(0, reachableAirports.Count)];

            Player.UnlockAirport(initialAirport1);
            Player.UnlockAirport(initialAirport2);

            // Init travellers in each airport
            Auxiliary.InitTravellersInAirports();

        }

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

                HashSet<Airplane> fullAirplanes = new HashSet<Airplane>();

                while (_info.savedAirports[origAirport.Name].TravellersToAirport[objAirport] > 0)
                {
                    Flight flight;

                    (Airplane objAirplane, Airport nextHop) = _info.savedAirports[origAirport.Name].FindHopForTravellersToAirport(objAirport);

                    if (objAirplane is null || nextHop is null)
                        break;

                    if (fullAirplanes.Contains(objAirplane))
                        break;

                    if (airportFlights[objAirport].Keys.Contains(objAirplane))
                    {
                        flight = airportFlights[objAirport][objAirplane];
                    }
                    else if (airplaneInFlight.Contains(objAirplane))
                    {
                        break;
                    }
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

                    if (flight.Full)
                        fullAirplanes.Add(objAirplane);
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