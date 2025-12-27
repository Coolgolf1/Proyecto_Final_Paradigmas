using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class Airport : MonoBehaviour, IUpgradable, IObject
{
    // Read-only from outside
    public string Id { get; private set; }
    public string Name { get; private set; }
    public int NumberOfRunways { get; private set; }
    public int ReceivedTravellers { get; private set; }
    public Location Location { get; private set; }
    public Levels Level { get; private set; }
    public int Capacity { get; private set; }
    public bool Unlocked { get; private set; } = false;
    public double Phase { get; private set; }
    public bool PriorityOn { get; set; }

    // Collections
    public List<Airplane> Hangar { get; } = new List<Airplane>();

    public Dictionary<Airport, int> TravellersToAirport { get; } = new Dictionary<Airport, int>();

    private float _unlockTime;

    // Objects/Dependencies
    private InfoSingleton _info = InfoSingleton.GetInstance();
    private EconomyManager _economy = EconomyManager.GetInstance();
    private GameMaster _gm = GameMaster.GetInstance();

    private InputAction _clickAction;
    private Camera _cam;

    private UnityEvent _advancePhase = new UnityEvent();
    private UnityEvent _reducePhase = new UnityEvent();

    // Serialize Field
    [SerializeField] private Light statusLight;
    private int _lightInt;

    // AI Spawner
    private System.Random _rand = new System.Random();
    private float _phaseTimer = 0f;
    private const float SpawnJitter = 0.5f;
    private float _nextSpawnTime = 0f;

    public void Initialise(string id, string name, Location location, int numberOfRunways = 2)
    {
        Id = id;
        Name = name;
        NumberOfRunways = numberOfRunways;
        ReceivedTravellers = 0;
        Location = location;

        Capacity = -GameConstants.maxTravellersInAirport;
        Level = Levels.Basic;

        gameObject.SetActive(false);

        GameEvents.OnAirportUnlock.AddListener(UpdateCapacity);

        Phase = Phases.Easy;
        SetNextEventTime();
        _nextSpawnTime = Time.time + GetSpawnIntervalForPhase();

        PriorityOn = false;
    }

    private void SetNextEventTime()
    {
        switch (Phase)
        {
            case Phases.Easy:
                _phaseTimer = UnityEngine.Random.Range(50f, 80f);
                break;

            case Phases.Medium:
                _phaseTimer = UnityEngine.Random.Range(120f, 210f);
                break;

            case Phases.Hard:
                _phaseTimer = UnityEngine.Random.Range(10f, 25f);
                break;

            case Phases.Surge:
                _phaseTimer = UnityEngine.Random.Range(5f, 10f);
                break;
        }
    }

    private void PhaseDirectorUpdate()
    {
        _phaseTimer -= Time.deltaTime;

        if (_phaseTimer <= 0)
        {
            if (Phase == Phases.Surge)
            {
                ReducePhase();
            }
            else if (Phase == Phases.Hard)
            {
                int choice = UnityEngine.Random.Range(0, 10);

                if (choice > 8)
                {
                    AdvancePhase();
                } else if (choice < 6)
                {
                    ReducePhase();
                }
            }
            else
            {
                AdvancePhase();
            }

            SetNextEventTime();
        }
    }

    private float GetSpawnIntervalForPhase()
    {
        switch (Phase)
        {
            case Phases.Easy:
                return 3.0f;

            case Phases.Medium:
                return 2.75f;

            case Phases.Hard:
                return 2.5f;

            case Phases.Surge:
                return 2.0f;

            default:
                return 3.0f;
        }
    }

    private void HandleSpawning()
    {
        if (Time.time >= _nextSpawnTime)
        {
            SpawnTravellers(Phase);

            float baseInterval = GetSpawnIntervalForPhase();

            float randomVariance = UnityEngine.Random.Range(-SpawnJitter, SpawnJitter);
            float interval = Mathf.Max(0.1f, baseInterval + randomVariance);

            _nextSpawnTime = Time.time + interval;
        }
    }

    public void AdvancePhase()
    {
        switch (Phase)
        {
            case Phases.Easy:
                Phase = Phases.Medium;
                break;

            case Phases.Medium:
                Phase = Phases.Hard;
                break;

            case Phases.Hard:
                Phase = Phases.Surge;
                break;
        }
    }

    public void ReducePhase()
    {
        switch (Phase)
        {
            case Phases.Easy:
                Phase = Phases.Easy;
                break;

            case Phases.Medium:
                Phase = Phases.Medium;
                break;

            case Phases.Hard:
                Phase = Phases.Hard;
                break;

            case Phases.Surge:
                Phase = Phases.Hard;
                break;
        }
    }

    private void UpdateLight()
    {
        int totalTravellers = TravellersToAirport.Values.Sum();

        float ratio = (float)totalTravellers / Capacity;

        if (Capacity != 0)
        {

            if (ratio < 0.5)
            {
                statusLight.color = Color.green;
                _lightInt = 1;

                if (_lightInt != 1)
                {
                    if (_info.playerCamera.GetComponent<PlayerMovement>() is SpaceCamera camera)
                    {
                        camera.DeactivateAlertMusic();
                    }
                }
            }
            else if (ratio < 0.7)
            {
                statusLight.color = Color.orange;
                if (_lightInt == 1)
                {
                    _info.notificationSystem.AddNotification($"{Name} is over 50% capacity", "warning", "orange");
                }
                _lightInt = 2;
            }
            else
            {
                statusLight.color = Color.red;
                if (_lightInt != 3)
                {
                    _info.notificationSystem.AddNotification($"{Name} is over 70% capacity", "alert", "red");
                }


                if (_info.playerCamera.GetComponent<PlayerMovement>() is SpaceCamera camera && _lightInt != 3)
                {
                    camera.ActivateAlertMusic();
                }

                _lightInt = 3;
            }
        }
    }

    public void Unlock()
    {
        Unlocked = true;
        _unlockTime = Time.time;
        gameObject.SetActive(Unlocked);

        GameEvents.OnAirportUnlock?.Invoke();
    }

    public void Upgrade()
    {
        if (Level < Levels.Elite)
            Level++;

        Capacity = Capacity + (int)(Capacity * (int)Level * GameConstants.AirportTravellersUpgrade);
    }

    public void SetCapacity(int capacity)
    {
        Capacity = capacity;
    }

    public void UpdateCapacity()
    {
        Capacity += GameConstants.maxTravellersInAirport + (int)(GameConstants.maxTravellersInAirport * (int)Level * GameConstants.AirportTravellersUpgrade);
    }

    public void Awake()
    {
        _clickAction = InputSystem.actions.FindAction("Click");
        _cam = _info.playerCamera;
        ReceivedTravellers = 0;
        //UIEvents.OnMainMenuEnter.AddListener(_clickAction.Disable);
        UIEvents.OnEndGameEnter.AddListener(CleanUI);
        //UIEvents.OnPlayEnter.AddListener(_clickAction.Enable);
        UIEvents.OnAirplaneStoreEnter.AddListener(_clickAction.Disable);
        UIEvents.OnAirplaneStoreExit.AddListener(_clickAction.Enable);
        UIEvents.OnRouteStoreEnter.AddListener(_clickAction.Disable);
        UIEvents.OnRouteStoreExit.AddListener(_clickAction.Enable);
    }

    public void CleanUI()
    {
        _info.airportUI.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _clickAction.performed += OnClickAirport;
        //_clickAction.Enable();
    }

    private void OnDisable()
    {
        _clickAction.performed -= OnClickAirport;
        //_clickAction.Disable();
    }

    private void OnClickAirport(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray ray = _cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Detecta colisión con este objeto
            if (hit.collider.gameObject == gameObject)
            {
                _info.flightUI.gameObject.SetActive(false);
                _info.airportUI.gameObject.SetActive(true);
                _info.airportUI.ShowAirport(this);
                List<Route> routes = _info.GetRoutesOfAirport(this);
                foreach (Route route in routes)
                {
                    route.LitRoute();
                }
            }
        }
    }

    public void InitTravellers(bool mainMenuGame = false)
    {
        foreach (Airport airportDest in _info.savedAirports.Values)
        {
            if (airportDest != this)
            {
                TravellersToAirport.Add(airportDest, 0);
            }
        }

        if (!mainMenuGame)
            return;

        foreach (Airport airport in _info.savedAirports.Values)
        {
            if (airport != this)
            {
                TravellersToAirport[airport] = _rand.Next(GameConstants.minTravellersCreatedInAirport, GameConstants.maxTravellersCreatedInAirport);
            }
        }
    }

    public void SpawnTravellers(double multiplier)
    {
        System.Random rand = new System.Random();

        foreach (Airport airport in Player.UnlockedAirports)
        {
            if (airport != this)
            {
                //Debug.Log((int)(rand.Next(GameConstants.minTravellersRandom, GameConstants.maxTravellersRandom) * multiplier));
                if (multiplier < airport.Phase)
                {
                    TravellersToAirport[airport] += (int)(rand.Next(GameConstants.minTravellersRandom, GameConstants.maxTravellersRandom) * multiplier);
                }
                else
                {
                    TravellersToAirport[airport] += (int)(rand.Next(GameConstants.minTravellersRandom, GameConstants.maxTravellersRandom) * airport.Phase);
                }

            }
        }
    }

    public (Airplane, Airport) FindHopForTravellersToAirport(Airport objectiveAirport)
    {
        // Calculate optimal path for plane
        (Airplane nextAirplane, List<Airport> path) = RouteAssigner.Dijkstra(this, objectiveAirport);

        if (nextAirplane is null)
        {
            //Debug.Log("Cannot find next airplane of path for travellers.");
            return (null, null);
        }

        if (path is null)
        {
            //Debug.Log("Cannot find path for travellers.");
            return (null, null);
        }

        // Get next hop of airplane
        Airport airport = _info.GetAirportOfAirplane(nextAirplane);

        if (airport is not null)
        {
            return (nextAirplane, RouteAssigner.GetNextHop(path));
        }
        else
        {
            return (null, null);
        }
    }

    public void TrackTakeOff(Flight flight)
    {
        flight.TakeOffEvent += HandleTakeOff;
    }

    public void TrackLanding(Flight flight)
    {
        flight.LandedEvent += HandleLanding;
    }

    public Airport GetNotEmptyAirportForAirplane(Airplane airplane)
    {
        foreach (Airport airport in GetReachableAirportsForAirplane(airplane))
        {
            // Check airport is not the same as origin
            if (this == airport)
                continue;

            // Check airport has remaining travellers
            int travellers = airport.TravellersToAirport.Values.Sum();
            if (travellers <= 0)
                continue;

            // Check airport doesn't have any airplanes in hangar
            //if (airport.Hangar.Count > 0)
            //    continue;

            // If a plane is already going there and its capacity is enough, no need to take plane
            bool airplaneGoing = false;
            foreach (Airplane tempAirplane in _info.airplanes)
            {
                Flight flight = _info.GetFlightOfAirplane(tempAirplane);

                // Check if flight exists
                if (flight == null)
                    continue;

                // Check if airplane going to airport
                if (flight.AirportDest != airport && !_info.airplanesGoingFromEmptyAirport[airport].Contains(tempAirplane))
                    continue;

                // Check if airplane has enough capacity
                if (airplane.Capacity < travellers)
                    continue;

                airplaneGoing = true;
                break;
            }

            // Check if a plane is not already going
            if (airplaneGoing)
                continue;

            return airport;
        }

        return null;
    }

    public List<Airport> GetReachableAirportsForAirplane(Airplane airplane)
    {
        List<Airport> availableAirports = new List<Airport>();
        foreach (Airport airport in TravellersToAirport.Keys)
        {
            double distance = Auxiliary.GetDirectDistanceBetweenAirports(this, airport);
            if (distance < airplane.Range)
            {
                availableAirports.Add(airport);
            }

        }

        return availableAirports;
    }

    public List<Airport> GetReachableAirportsForRange(double range)
    {
        List<Airport> availableAirports = new List<Airport>();
        foreach (Airport airport in _info.savedAirports.Values)
        {
            if (airport == this)
                continue;

            double distance = Auxiliary.GetDirectDistanceBetweenAirports(this, airport);
            if (distance < range)
            {
                availableAirports.Add(airport);
            }

        }

        return availableAirports;
    }

    public (Airplane, Airport, Airport) GetHopFromEmptyAirport(Airplane airplane)
    {
        Airport notEmptyAirport = GetNotEmptyAirportForAirplane(airplane);

        if (notEmptyAirport == null)
        {
            return (null, null, null);
        }

        (Airplane objAirplane, Airport nextHop) = FindHopForTravellersToAirport(notEmptyAirport);

        if (objAirplane is null)
        {
            return (null, null, null);
        }

        int travellers = 0;
        List<Airport> availableAirports = GetReachableAirportsForAirplane(objAirplane);

        foreach (Airport airport in availableAirports)
        {
            travellers += TravellersToAirport[airport];
        }

        if (travellers > 0)
            return (null, null, null);


        return (objAirplane, nextHop, notEmptyAirport);
    }

    public void CheckMaxPassengers()
    {
        int count = TravellersToAirport.Values.Sum();

        if (count > Capacity)
        {
            if (_info.playerCamera.GetComponent<PlayerMovement>() is SpaceCamera camera)
            {
                camera.SetAirport(this);

            }
            _gm.ChangeState(_gm.End);
        }
    }

    public void HandleTakeOff(object sender, EventArgs e)
    {
        Flight flight = (Flight)sender;
        Hangar.Remove(flight.Airplane);
    }

    public void ReceivePassengers(int passengers, Airport objAirport, Flight flight)
    {
        if (objAirport != this)
        {
            TravellersToAirport[objAirport] += passengers;
        }
        else
        {
            ReceivedTravellers += passengers;
            _info.totalTravellersReceived += passengers;

            // Give coins to user
            _economy.SaveCoins(passengers, Auxiliary.GetDirectDistanceBetweenAirports(this, flight.AirportOrig));
        }
    }

    public void HandleLanding(object sender, EventArgs e)
    {
        Flight flight = (Flight)sender;

        Hangar.Add(flight.Airplane);

        Dictionary<Airplane, Flight> createdFlights = new Dictionary<Airplane, Flight>();

        // For all travellers in origin airport, assign each group of travellers an airplane
        List<Airport> origKeys = new List<Airport>(TravellersToAirport.Keys);

        PriorityQueue<Airport> airportQueue = new PriorityQueue<Airport>();

        foreach (Airport airport in TravellersToAirport.Keys)
        {
            // Min priority queue, so negative number
            if (airport.PriorityOn)
                airportQueue.Enqueue(airport, -100000000);
            else
                airportQueue.Enqueue(airport, -TravellersToAirport[airport]);
        }

        while (airportQueue.Count > 0)
        {
            Airport airport = airportQueue.Dequeue();

            if (airport == this)
                continue;

            // If no travellers to airport, skip airport
            if (TravellersToAirport[airport] <= 0)
                continue;

            while (TravellersToAirport[airport] > 0)
            {
                Flight newFlight;

                (Airplane objAirplane, Airport nextHop) = FindHopForTravellersToAirport(airport);

                if (objAirplane is null || nextHop is null)
                {
                    //Debug.Log(objAirplane);
                    break;
                }

                if (createdFlights.Keys.Contains(objAirplane))
                {
                    newFlight = createdFlights[objAirplane];
                }
                else
                {
                    GameObject flightGO = new GameObject();
                    flightGO.name = $"{Name}-{nextHop.Name}";
                    newFlight = flightGO.AddComponent<Flight>();

                    newFlight.Initialise(this, nextHop, _info.savedRoutes[$"{Name}-{nextHop.Name}"], objAirplane);
                    _info.flights.Add(newFlight);
                    createdFlights[objAirplane] = newFlight;
                }

                newFlight.Embark(TravellersToAirport[airport], airport);
            }
        }

        foreach (Flight tempFlight in createdFlights.Values)
        {
            tempFlight.StartFlight();
        }

        // If origin airport has no travellers, take any airplane to another airport with passengers
        GameEvents.OnPlaneLandedAndBoarded?.Invoke();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        transform.position = Location.coords;

        Vector3 up = (transform.position - Vector3.zero).normalized;

        // Elige un vector "forward" perpendicular a "up".
        // Si "up" está cerca de Vector3.up, usa Vector3.forward, si no, usa Vector3.up.
        Vector3 forward = Vector3.Cross(up, Vector3.right);
        if (forward == Vector3.zero)
            forward = Vector3.Cross(up, Vector3.forward);

        transform.rotation = Quaternion.LookRotation(forward, up);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!Unlocked)
            return;

        HandleSpawning();

        PhaseDirectorUpdate();

        CheckMaxPassengers();

        UpdateLight();
    }
}