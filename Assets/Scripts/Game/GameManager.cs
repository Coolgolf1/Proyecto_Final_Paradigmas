using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject airportPrefab;



    [SerializeField] private GameObject earth;

    [SerializeField] private AirportUI airportUI;
    [SerializeField] private FlightUI flightUI;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AirplaneSpawner airplaneSpawner;
    [SerializeField] private GameObject routePrefab;

    [SerializeField] private NotificationSystem notificationSystem;


    private InfoSingleton _info = InfoSingleton.GetInstance();
    private AirplaneFactory _airplaneFactory = AirplaneFactory.GetInstance();
    private EconomyManager _economy = EconomyManager.GetInstance();
    private Init _init;
    private bool _mainMenuGame = false;

    private System.Random _rand = new System.Random();

    private InputAction _clickAction;

    // Expansion
    public double Phase { get; private set; }
    private double _currentMaxExpansion = 2000;
    private float _nextUnlockTime = 0f;
    private float _nextLaunchTime = 0f;
    private float _phaseTimer;

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

        _info.notificationSystem = notificationSystem;

        // Initialise initialliser
        _init = gameObject.AddComponent<Init>();

        _clickAction = InputSystem.actions.FindAction("Click");

        // Start game or main menu game
        GameEvents.OnMainMenuEnter.AddListener(StartMainMenuGame);
        GameEvents.OnMainMenuExit.AddListener(StopMainMenuGame);

        // Start game if game or main menu game
        GameEvents.OnPlayEnter.AddListener(StartGame);
        GameEvents.OnMainMenuEnter.AddListener(StartGame);

        // Reset game if end or exit main menu game
        UIEvents.OnEndGameExit.AddListener(ResetGame);
        GameEvents.OnMainMenuExit.AddListener(ResetGame);

        FlightLauncher.InitFlightLauncher();

        Phase = Phases.Easy;

        _phaseTimer = GetPhaseTimer();
        UpdateExpansionRules();
        _nextUnlockTime = Time.time + 5f;
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
        //_economy.SetCoins(2000000);
        Player.Restart();
        Phase = Phases.Easy;
        _phaseTimer = _rand.Next(50, 70);
        _currentMaxExpansion = 2000;
        _nextUnlockTime = Time.time + _phaseTimer;
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

        // Initialise list of empty airports once airports have loaded
        _info.InitEmptyAirportList();

        // Save data of routes
        if (_mainMenuGame)
        {
            _clickAction.Disable();
            InitMainMenuAirplanes(earth.transform);

            _init.SaveDataOfRoutes(routePrefab, earth.transform);
            foreach (Airport airport in _info.savedAirports.Values)
            {
                Player.UnlockAirport(airport);
                airport.SetCapacity(1000000000);
            }

            // Init travellers in each airport
            Auxiliary.InitTravellersInAirports(mainMenuGame: true);
        }
        else
        {

            _economy.SetCoins(1000000);
            _clickAction.Enable();

            _info.ResetRoutes();

            System.Random _rand = new System.Random();

            List<Airport> allAirports = _info.savedAirports.Values.ToList();

            List<Airport> validStarts = new List<Airport>();

            foreach (Airport airport in allAirports)
            {
                // Check if this airport has at least one reachable neighbor
                if (airport.GetReachableAirportsForRange(GameConstants.smallRange).Count > 0)
                {
                    validStarts.Add(airport);
                }
            }

            if (validStarts.Count == 0)
            {
                return;
            }

            Airport initialAirport1 = validStarts[_rand.Next(0, validStarts.Count)];

            List<Airport> reachableAirports = initialAirport1.GetReachableAirportsForRange(GameConstants.smallRange);

            Airport initialAirport2 = reachableAirports[_rand.Next(0, reachableAirports.Count)];

            Player.UnlockAirport(initialAirport1);
            Player.UnlockAirport(initialAirport2);
            if (playerCamera.GetComponent<PlayerMovement>() is SpaceCamera spaceCamera)
            {
                _info.notificationSystem.AddNotification($"Unlocked new airport: {initialAirport1.Name}.", "airport", "blue", delegate { spaceCamera.SetAirport(initialAirport1); });
                _info.notificationSystem.AddNotification($"Unlocked new airport: {initialAirport2.Name}.", "airport", "blue", delegate { spaceCamera.SetAirport(initialAirport2); });
            }
            else
            {
                _info.notificationSystem.AddNotification($"Unlocked new airport: {initialAirport1.Name}.", "airport", "blue");
                _info.notificationSystem.AddNotification($"Unlocked new airport: {initialAirport2.Name}.", "airport", "blue");
            }
            
            
            

            if (_info.playerCamera.GetComponent<PlayerMovement>() is SpaceCamera camera)
            {
                camera.SetAirport(initialAirport1);

            }

            // Init travellers in each airport
            Auxiliary.InitTravellersInAirports();

        }

        // Load the real distances from dataset
        Auxiliary.LoadRouteDistances(_info.savedRoutes);

        // Calculate initial Dijkstra Graph
        Auxiliary.CalculateDijkstraGraph();

        //foreach (List<Edge> edges in DijkstraGraph.graph.Values)
        //{
        //    foreach (Edge edge in edges)
        //    {
        //        Debug.Log(edge);
        //    }
        //}

        List<Airport> destinations = Player.UnlockedAirports;

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

                if (origAirport.TravellersToAirport[objAirport] <= 0)
                {
                    continue;
                }

                while (origAirport.TravellersToAirport[objAirport] > 0)
                {
                    Flight flight;

                    (Airplane objAirplane, Airport nextHop) = origAirport.FindHopForTravellersToAirport(objAirport);

                    if (objAirplane is null || nextHop is null)
                        break;

                    if (airportFlights[origAirport].Keys.Contains(objAirplane))
                    {
                        flight = airportFlights[origAirport][objAirplane];
                    }
                    else
                    {
                        GameObject flightGO = new GameObject();
                        flightGO.name = $"{origAirport.Name}-{nextHop.Name}";
                        flight = flightGO.AddComponent<Flight>();

                        flight.Initialise(origAirport, nextHop, _info.savedRoutes[$"{origAirport.Name}-{nextHop.Name}"], objAirplane);

                        _info.flights.Add(flight);
                        airportFlights[origAirport][objAirplane] = flight;

                        airplaneInFlight.Add(objAirplane);
                    }

                    flight.Embark(origAirport.TravellersToAirport[objAirport], objAirport);
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

    public List<Airport> GetPossibleAirports()
    {
        List<Airport> candidates = new List<Airport>();
        List<Airport> unlockedAirports = Player.UnlockedAirports;

        foreach (Airport candidate in _info.savedAirports.Values)
        {
            if (unlockedAirports.Contains(candidate))
                continue;

            bool isReachable = false;

            foreach (Airport myAirport in unlockedAirports)
            {
                double dist = Auxiliary.GetDirectDistanceBetweenAirports(candidate, myAirport);

                if (dist <= _currentMaxExpansion)
                {
                    isReachable = true;
                    break;
                }
            }

            if (isReachable)
                candidates.Add(candidate);
        }

        return candidates;
    }

    private void TriggerAirportExpansion()
    {
        List<Airport> possibleAirports = GetPossibleAirports();

        if (possibleAirports.Count <= 0)
        {
            _currentMaxExpansion += 2000;
            return;
        }

        Airport newAirport = possibleAirports[_rand.Next(possibleAirports.Count)];
        Player.UnlockAirport(newAirport);

        //Debug.Log($"Unlocked new airport: {newAirport}.");

        if (playerCamera.GetComponent<PlayerMovement>() is SpaceCamera camera)
        {
            _info.notificationSystem.AddNotification($"Unlocked new airport: {newAirport.Name}.", "airport", "blue", delegate { camera.SetAirport(newAirport); });
        }
        else {
            _info.notificationSystem.AddNotification($"Unlocked new airport: {newAirport.Name}.", "airport", "blue");
        }


    }

    private float GetPhaseTimer()
    {
        double randomNoise = _rand.Next(-5, 5);

        double phaseDuration;

        switch (Phase)
        {
            case Phases.Medium:
                phaseDuration = _rand.Next(100, 120);
                break;

            case Phases.Hard:
                phaseDuration = _rand.Next(50, 70);
                break;

            default:
                phaseDuration = _rand.Next(80, 100);
                break;
        }

        return (float)(phaseDuration + randomNoise);
    }

    private void UpdateDirector()
    {
        _phaseTimer -= Time.deltaTime;

        if (_phaseTimer <= 0)
        {
            if (Phase == Phases.Hard)
            {
                Phase = Phases.Medium;
                _phaseTimer = GetPhaseTimer();
                UpdateExpansionRules();
            }
            else
            {
                AdvancePhase();
            }
        }
    }

    private void AdvancePhase()
    {
        switch (Phase)
        {
            case Phases.Easy:
                Phase = Phases.Medium;
                break;

            case Phases.Medium:
                Phase = Phases.Hard;
                break;
        }
        //Debug.Log($"Game phase: {Phases.Surge}");
        _phaseTimer = GetPhaseTimer();
        UpdateExpansionRules();
    }

    private void UpdateExpansionRules()
    {
        switch (Phase)
        {
            case Phases.Easy:
                _currentMaxExpansion = 3000;
                break;

            case Phases.Medium:
                _currentMaxExpansion = 5000;
                break;

            case Phases.Hard:
                _currentMaxExpansion = 10000;
                break;
        }
    }

    private float GetSpawnIntervalForPhase()
    {
        switch (Phase)
        {
            case Phases.Easy:
                return _rand.Next(120, 150);

            case Phases.Medium:
                return _rand.Next(90, 120);

            case Phases.Hard:
                return _rand.Next(60, 90);

            default:
                return 100f;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_mainMenuGame)
            return;

        UpdateDirector();

        if (Time.time > _nextUnlockTime)
        {
            TriggerAirportExpansion();
            float interval = GetSpawnIntervalForPhase();
            _nextUnlockTime = Time.time + interval;
        }

        if (Time.time > _nextLaunchTime)
        {
            FlightLauncher.LaunchNewFlights();
            _nextLaunchTime = Time.time + 2f;
        }
    }
}