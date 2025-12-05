using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
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

    // Collections
    public List<Airplane> Hangar { get; } = new List<Airplane>();

    public Dictionary<Airport, int> TravellersToAirport { get; } = new Dictionary<Airport, int>();

    // Objects/Dependencies
    private InfoSingleton _info = InfoSingleton.GetInstance();
    private EconomyManager _economy = EconomyManager.GetInstance();

    private InputAction _clickAction;
    private Camera _cam;

    // Serialize Field
    [SerializeField] public GameObject modelPrefab;

    public void Initialise(string id, string name, Location location, int numberOfRunways = 2)
    {
        Id = id;
        Name = name;
        NumberOfRunways = numberOfRunways;
        ReceivedTravellers = 0;
        Location = location;

        Level = Levels.Basic;
    }

    public void Upgrade()
    {
        if (Level < Levels.Elite)
            Level++;
    }

    public void Awake()
    {
        _clickAction = InputSystem.actions.FindAction("Click");
        _cam = _info.playerCamera;
        ReceivedTravellers = 0;
        UIEvents.OnStoreEnter.AddListener(_clickAction.Disable);
        UIEvents.OnStoreExit.AddListener(_clickAction.Enable);
    }

    private void OnEnable()
    {
        _clickAction.performed += OnClickAirport;
        _clickAction.Enable();
    }

    private void OnDisable()
    {
        _clickAction.performed -= OnClickAirport;
    }

    private void OnClickAirport(InputAction.CallbackContext ctx)
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray ray = _cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Detecta colisión con este objeto
            if (hit.collider.gameObject == this.gameObject)
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

    public void InitTravellers()
    {
        foreach (Airport airportDest in _info.savedAirports.Values)
        {
            if (airportDest != this)
            {
                TravellersToAirport.Add(airportDest, 0);
            }
        }

        System.Random rand = new System.Random();

        foreach (Airport airport in _info.savedAirports.Values)
        {
            if (airport != this)
            {
                TravellersToAirport[airport] = rand.Next(GameConstants.minTravellersCreatedInAirport, GameConstants.maxTravellersCreatedInAirport);
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
            if (airport.Hangar.Count > 0)
                continue;

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

    public void HandleTakeOff(object sender, EventArgs e)
    {
        Flight flight = (Flight)sender;
        Hangar.Remove(flight.Airplane);
    }

    public void ReceivePassengers(int passengers, Airport objAirport)
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
            _economy.SaveCoins(passengers);
        }
    }

    public Dictionary<Airplane, Flight> SaveEmptyAirportFlights(Dictionary<Airplane, Flight> createdFlights)
    {
        Dictionary<Airport, int> airportsGoing = new Dictionary<Airport, int>();

        // FIX THIS LOGIC =========================================================================================
        List<Airplane> airplanesInHangar = new List<Airplane>();
        foreach (Airport airport in _info.savedAirports.Values)
        {
            airportsGoing.Add(airport, 0);

            airplanesInHangar.Concat(airport.Hangar);
        }

        foreach (Airplane airplane in airplanesInHangar)
        {
            (Airplane emptyAirplane, Airport emptyHop, Airport emptyAirport) = GetHopFromEmptyAirport(airplane);

            if (emptyAirplane is not null && emptyHop is not null)
            {
                if (airportsGoing[emptyHop] * airplane.Capacity > TravellersToAirport[emptyHop])
                    continue;

                Debug.Log($"HELLO, I'm {airplane}, {airportsGoing[emptyHop] * airplane.Capacity}");

                Flight emptyFlight;

                GameObject flightGO = new GameObject();
                flightGO.name = $"{Name}-{emptyHop.Name}";
                emptyFlight = flightGO.AddComponent<Flight>();
                emptyFlight.Initialise(this, emptyHop, _info.savedRoutes[$"{Name}-{emptyHop.Name}"], emptyAirplane);
                _info.flights.Add(emptyFlight);
                createdFlights[emptyAirplane] = emptyFlight;

                _info.airplanesGoingFromEmptyAirport[emptyAirport].Add(emptyAirplane);

                airportsGoing[emptyHop] += 1;
            }
        }

        return createdFlights;
    }

    public void HandleLanding(object sender, EventArgs e)
    {
        Flight flight = (Flight)sender;

        if (_info.airplanesGoingFromEmptyAirport.Keys.Contains(this))
            _info.airplanesGoingFromEmptyAirport[this].Remove(flight.Airplane);

        Hangar.Add(flight.Airplane);

        Dictionary<Airplane, Flight> createdFlights = new Dictionary<Airplane, Flight>();

        // If origin airport has no travellers, take any airplane to another airport with passengers
        createdFlights = SaveEmptyAirportFlights(createdFlights);

        // For all travellers in origin airport, assign each group of travellers an airplane
        List<Airport> origKeys = new List<Airport>(TravellersToAirport.Keys);

        PriorityQueue<Airport> airportQueue = new PriorityQueue<Airport>();

        foreach (Airport airport in TravellersToAirport.Keys)
        {
            airportQueue.Enqueue(airport, -TravellersToAirport[airport]);
        }

        List<Airplane> airplaneInFlight = new List<Airplane>();

        while (airportQueue.Count > 0)
        {
            Airport airport = airportQueue.Dequeue();

            if (airport == this)
                continue;

            // If no travellers to airport, skip airport
            if (TravellersToAirport[airport] <= 0)
                continue;

            HashSet<Airplane> usedThisIteration = new HashSet<Airplane>();

            while (TravellersToAirport[airport] > 0)
            {
                Flight newFlight;

                (Airplane objAirplane, Airport nextHop) = FindHopForTravellersToAirport(airport);

                if (objAirplane is null || nextHop is null)
                    break;

                if (usedThisIteration.Contains(objAirplane))
                {
                    break; // or: try to pick another plane
                }
                usedThisIteration.Add(objAirplane);

                if (createdFlights.Keys.Contains(objAirplane))
                {
                    newFlight = createdFlights[objAirplane];
                }
                else if (airplaneInFlight.Contains(objAirplane))
                    continue;
                else
                {
                    GameObject flightGO = new GameObject();
                    flightGO.name = $"{Name}-{nextHop.Name}";
                    newFlight = flightGO.AddComponent<Flight>();

                    newFlight.Initialise(this, nextHop, _info.savedRoutes[$"{Name}-{nextHop.Name}"], objAirplane);
                    _info.flights.Add(newFlight);
                    createdFlights[objAirplane] = newFlight;

                    airplaneInFlight.Add(objAirplane);
                }

                newFlight.Embark(TravellersToAirport[airport], airport);

                if (newFlight.Full)
                {
                    break;
                }
            }
        }

        foreach (Flight tempFlight in createdFlights.Values)
        {
            tempFlight.StartFlight();
        }

        //FlightLauncher.LaunchFlights();
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
    }
}