using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Airport : MonoBehaviour
{
    public string id;
    public string Name;
    public int numberOfRunways;

    public List<Airplane> hangar = new List<Airplane>();

    public int receivedTravellers;
    private InfoSingleton info = InfoSingleton.GetInstance();
    public Dictionary<Airport, int> TravellersToAirport { get; private set; }

    [SerializeField]
    public Location location;
    public GameObject modelPrefab;

    private InputAction clickAction;
    private Camera cam;

    public void Awake()
    {
        TravellersToAirport = new Dictionary<Airport, int>();
        clickAction = InputSystem.actions.FindAction("Click");
        cam = info.playerCamera;
        receivedTravellers = 0;
    }
    private void OnEnable()
    {
        clickAction.performed += OnClickAirport;
        //clickAction.Enable();
    }

    private void OnDisable()
    {
        clickAction.performed -= OnClickAirport;
        //clickAction.Disable();
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
                info.flightUI.gameObject.SetActive(false);
                info.airportUI.gameObject.SetActive(true);
                info.airportUI.ShowAirport(this);
            }

        }
    }

    public void InitTravellers()
    {
        foreach (Airport airport in info.savedAirports.Values.ToList())
        {
            if (airport != this)
            {
                TravellersToAirport.Add(airport, 0);
            }
        }

        //foreach (Airport airport in info.savedAirports.Values.ToList())
        //{
        //    if (airport != this)
        //    {
        //        // TODO: CHANGE LATER TO RANDOM OR SOMETHING DIFFERENT ======================================
        //        TravellersToAirport[airport] = 10;
        //    }
        //}
    }

    public (Airplane, Airport) FindAirplaneForTravellersToAirport(Airport objectiveAirport)
    {
        (Airplane nextAirplane, List<Airport> path) = RouteAssigner.Dijkstra(info.DijkstraGraph, this, objectiveAirport);

        if (nextAirplane is null)
        {
            Debug.Log("Cannot find next airplane of path for travellers.");
            return (null, null);
        }

        if (path is null)
        {
            Debug.Log("Cannot find path for travellers.");
            return (null, null);
        }

        // ======== USE PATH LATER ON TO CHOOSE AIRPLANES IF NECESSARY ===================

        // INCLUDE THIS IN DIJKSTRA LATER ON ==============================================
        Airport airport = info.GetAirportOfAirplane(nextAirplane);

        if (airport is not null)
        {
            return (nextAirplane, RouteAssigner.GetNextHop(path));
        }
        else
        {
            return (null, null);
        }
    }

    public void TrackFlight(Flight flight)
    {
        flight.LandedEvent += HandleLanding;
    }

    public void AssignTravellersToNextFlightOfAirplane(Flight flight, Airplane objAirplane, Airport nextHop, Airport objAirport)
    {
        int occupiedCapacity = flight.TravellersToAirport.Values.ToList().Sum();

        int remainingCapacity = objAirplane.Capacity - occupiedCapacity;

        int travellersInAirport = TravellersToAirport[objAirport];

        if (travellersInAirport <= remainingCapacity)
        {
            flight.TravellersToAirport[objAirport] += travellersInAirport;
            TravellersToAirport[objAirport] -= flight.TravellersToAirport[objAirport];
        }
        else
        {
            flight.TravellersToAirport[objAirport] += remainingCapacity;
            TravellersToAirport[objAirport] -= remainingCapacity;
        }
    }

    public Airport GetNotEmptyAirport()
    {
        foreach (Airport airport in info.savedAirports.Values)
        {
            // Check airport is not the same as origin
            if (this == airport)
                continue;

            // Check airport has remaining travellers
            int travellers = airport.TravellersToAirport.Values.Sum();
            if (travellers <= 0)
                continue;

            // Check airport doesn't have any airplanes in hangar
            if (airport.hangar.Count > 0)
                continue;

            // If a plane is already going there and its capacity is enough, no need to take plane
            bool airplaneGoing = false;
            foreach (Airplane airplane in info.airplanes)
            {
                Flight flight = info.GetFlightOfAirplane(airplane);

                // Check if flight exists
                if (flight == null)
                    continue;

                // Check if airplane going to airport
                if (flight.airportDest != airport && !info.airplanesGoingFromEmptyAirport[airport].Contains(airplane))
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

    public (Airplane, Airport, Airport) GetAirplaneAndAirportToEmptyAirport()
    {
        int travellers = TravellersToAirport.Values.Sum();

        if (travellers > 0)
            return (null, null, null);

        Airport emptyAirport = GetNotEmptyAirport();

        if (emptyAirport == null)
        {
            return (null, null, null);
        }

        (Airplane objAirplane, Airport nextHop) = FindAirplaneForTravellersToAirport(emptyAirport);

        return (objAirplane, nextHop, emptyAirport);
    }

    public void HandleLanding(object sender, EventArgs e)
    {
        Flight flight = (Flight)sender;

        if (info.airplanesGoingFromEmptyAirport.Keys.Contains(this))
            info.airplanesGoingFromEmptyAirport[this].Remove(flight.airplane);

        hangar.Add(flight.airplane);
        info.flights.Remove(flight);

        // UPDATE DIJKSTRA IN AIRPORT FOR NEW TRAVELLERS ======================================
        //info.CalculateDijkstraGraph();

        // If origin airport has no travellers, take any airplane to another airport with passengers
        (Airplane emptyAirplane, Airport emptyHop, Airport emptyAirport) = GetAirplaneAndAirportToEmptyAirport();

        if (emptyAirplane is not null && emptyHop is not null)
        {
            Flight emptyFlight = Auxiliary.CreateFlight(this, emptyHop, info.savedRoutes[$"{Name}-{emptyHop.Name}"], emptyAirplane);
            emptyFlight.StartFlight();
            info.airplanesGoingFromEmptyAirport[emptyAirport].Add(emptyAirplane);
            return;
        }


        Dictionary<Airplane, Flight> createdFlights = new Dictionary<Airplane, Flight>();

        // For all travellers in origin airport, assign each of the travellers an airplane
        var origKeys = new List<Airport>(TravellersToAirport.Keys);

        Queue<Airport> airportQueue = new Queue<Airport>(info.savedAirports.Values.ToList());

        while (airportQueue.Count > 0)
        {
            Airport airport = airportQueue.Dequeue();

            if (airport == this)
                continue;

            // If no travellers to airport, skip airport
            if (TravellersToAirport[airport] <= 0)
                continue;

            //Debug.Log($"{this}: {airport}-{TravellersToAirport[airport]}");

            HashSet<Airplane> usedThisIteration = new HashSet<Airplane>();

            while (TravellersToAirport[airport] > 0)
            {
                Flight newFlight;

                (Airplane objAirplane, Airport nextHop) = FindAirplaneForTravellersToAirport(airport);

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
                    newFlight = Auxiliary.CreateFlight(this, nextHop, info.savedRoutes[$"{Name}-{nextHop.Name}"], objAirplane);
                    createdFlights[objAirplane] = newFlight;
                }

                AssignTravellersToNextFlightOfAirplane(newFlight, objAirplane, nextHop, airport);
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

    //public void LaunchNewPlaneAfterLanding(Flight flight)
    //{
    //    Airplane airplane = flight.airplane;
    //    Flight newFlight = Auxiliary.CreateFlight(this, info.GetLandingAirportOfAirplane(airplane), info.GetRouteOfAirplane(airplane), airplane);

    //    newFlight.BoardFlight(TravellersToAirport);
    //    Instantiate(newFlight);
    //}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        transform.position = location.coords;

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