using System;
using System.Collections.Generic;
using System.Linq;
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
    private InputAction clickAction;
    private Camera cam;

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
        clickAction = InputSystem.actions.FindAction("Click");
        cam = _info.playerCamera;
        ReceivedTravellers = 0;
    }

    private void OnEnable()
    {
        clickAction.performed += OnClickAirport;
        clickAction.Enable();
    }

    private void OnDisable()
    {
        clickAction.performed -= OnClickAirport;
    }

    private void OnClickAirport(InputAction.CallbackContext ctx)
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Detecta colisión con este objeto
            if (hit.collider.gameObject == this.gameObject)
            {
                _info.flightUI.gameObject.SetActive(false);
                _info.airportUI.gameObject.SetActive(true);
                _info.airportUI.ShowAirport(this);
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
        //foreach (Airport airport in _info.savedAirports.Values)
        //{
        //    if (airport != this)
        //    {
        //        // TODO: CHANGE LATER TO RANDOM OR SOMETHING DIFFERENT ======================================
        //        TravellersToAirport[airport] = 10;
        //    }
        //}
    }

    public (Airplane, Airport) FindHopForTravellersToAirport(Airport objectiveAirport)
    {
        // Calculate optimal path for plane
        (Airplane nextAirplane, List<Airport> path) = RouteAssigner.Dijkstra(_info.DijkstraGraph, this, objectiveAirport);

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

    public Airport GetNotEmptyAirport()
    {
        foreach (Airport airport in _info.savedAirports.Values)
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
            foreach (Airplane airplane in _info.airplanes)
            {
                Flight flight = _info.GetFlightOfAirplane(airplane);

                // Check if flight exists
                if (flight == null)
                    continue;

                // Check if airplane going to airport
                if (flight.AirportDest != airport && !_info.airplanesGoingFromEmptyAirport[airport].Contains(airplane))
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

    public (Airplane, Airport, Airport) GetHopToEmptyAirport()
    {
        int travellers = TravellersToAirport.Values.Sum();

        if (travellers > 0)
            return (null, null, null);

        Airport emptyAirport = GetNotEmptyAirport();

        if (emptyAirport == null)
        {
            return (null, null, null);
        }

        (Airplane objAirplane, Airport nextHop) = FindHopForTravellersToAirport(emptyAirport);

        return (objAirplane, nextHop, emptyAirport);
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
            // GIVE COINS FOR EACH PASSENGER TAKEN TO CORRECT AIRPORT SUCCESSFULLY ======================================
        }
    }

    public void HandleLanding(object sender, EventArgs e)
    {
        Flight flight = (Flight)sender;

        if (_info.airplanesGoingFromEmptyAirport.Keys.Contains(this))
            _info.airplanesGoingFromEmptyAirport[this].Remove(flight.Airplane);

        Hangar.Add(flight.Airplane);

        // If origin airport has no travellers, take any airplane to another airport with passengers
        (Airplane emptyAirplane, Airport emptyHop, Airport emptyAirport) = GetHopToEmptyAirport();

        if (emptyAirplane is not null && emptyHop is not null)
        {
            Flight emptyFlight;

            GameObject flightGO = new GameObject();
            flightGO.name = $"{_info.savedAirports["Madrid"].Name}-{emptyHop.Name}";
            emptyFlight = flightGO.AddComponent<Flight>();
            emptyFlight.Initialise(this, emptyHop, _info.savedRoutes[$"{Name}-{emptyHop.Name}"], emptyAirplane);
            _info.flights.Add(emptyFlight);
            emptyFlight.StartFlight();

            _info.airplanesGoingFromEmptyAirport[emptyAirport].Add(emptyAirplane);
            return;
        }

        Dictionary<Airplane, Flight> createdFlights = new Dictionary<Airplane, Flight>();

        // For all travellers in origin airport, assign each of the travellers an airplane
        var origKeys = new List<Airport>(TravellersToAirport.Keys);

        Queue<Airport> airportQueue = new Queue<Airport>(_info.savedAirports.Values.ToList());

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
                    continue;

                if (usedThisIteration.Contains(objAirplane))
                {
                    break; // or: try to pick another plane
                }
                usedThisIteration.Add(objAirplane);

                if (createdFlights.Keys.Contains(objAirplane))
                {
                    newFlight = createdFlights[objAirplane];
                }
                else
                {
                    GameObject flightGO = new GameObject();
                    flightGO.name = $"{_info.savedAirports["Madrid"].Name}-{nextHop.Name}";
                    newFlight = flightGO.AddComponent<Flight>();

                    newFlight.Initialise(this, nextHop, _info.savedRoutes[$"{Name}-{nextHop.Name}"], objAirplane);
                    _info.flights.Add(newFlight);
                    createdFlights[objAirplane] = newFlight;
                }

                newFlight.Embark(TravellersToAirport[airport], airport);
            }

            if (TravellersToAirport[airport] > 0)
            {
                airportQueue.Enqueue(airport);
            }
        }

        foreach (Flight tempFlight in createdFlights.Values)
        {
            tempFlight.StartFlight();
        }
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