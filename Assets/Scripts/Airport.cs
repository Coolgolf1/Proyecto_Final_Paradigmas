using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Airport : MonoBehaviour
{
    public string id;
    public string Name;
    public int numberOfRunways;

    public List<Airplane> hangar = new List<Airplane>();

    public Dictionary<Airport, int> TravellersToAirport { get; private set; }

    [SerializeField]
    public Location location;
    public GameObject modelPrefab;

    public void Awake()
    {
        TravellersToAirport = new Dictionary<Airport, int>();
    }

    public void InitTravellers()
    {
        foreach (Airport airport in Info.savedAirports.Values.ToList())
        {
            if (airport != this)
            {
                TravellersToAirport.Add(airport, 0);
            }
        }

        //foreach (Airport airport in Info.savedAirports.Values.ToList())
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
        (Airplane nextAirplane, Airport nextHop) = RouteAssigner.Dijkstra(Info.DijkstraGraph, this, objectiveAirport);

        if (nextAirplane is null)
        {
            Debug.Log("Cannot find next airplane of path for travellers.");
            return (null, null);
        }

        if (nextHop is null)
        {
            Debug.Log("Cannot find next airport of path for travellers.");
            return (null, null);
        }


        // INCLUDE THIS IN DIJKSTRA LATER ON ==============================================
        Airport airport = Info.GetAirportOfAirplane(nextAirplane);

        if (airport is not null)
        {
            return (nextAirplane, nextHop);
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
            TravellersToAirport[objAirport] -= travellersInAirport;
        }
        else
        {
            flight.TravellersToAirport[objAirport] += (travellersInAirport - remainingCapacity);
            TravellersToAirport[objAirport] -= (travellersInAirport - remainingCapacity);
        }
    }

    public void HandleLanding(object sender, EventArgs e)
    {
        Flight flight = (Flight)sender;

        hangar.Add(flight.airplane);
        Info.flights.Remove(flight);

        // UPDATE DIJKSTRA IN AIRPORT FOR NEW TRAVELLERS ======================================
        //Info.CalculateDijkstraGraph();

        Dictionary<Airplane, Flight> createdFlights = new Dictionary<Airplane, Flight>();

        // For all travellers in origin airport, assign each of the travellers an airplane
        var origKeys = new List<Airport>(TravellersToAirport.Keys);
        foreach (Airport airport in origKeys)
        {
            // If no travellers to airport, skip airport
            if (TravellersToAirport[airport] <= 0)
            {
                continue;
            }

            //Debug.Log($"{this}: {airport}-{TravellersToAirport[airport]}");

            Flight newFlight;

            (Airplane objAirplane, Airport nextHop) = FindAirplaneForTravellersToAirport(airport);

            if (objAirplane is null || nextHop is null)
            {
                continue;
            }

            if (createdFlights.Keys.Contains(objAirplane))
            {
                newFlight = createdFlights[objAirplane];

            }
            else
            {
                newFlight = Auxiliary.CreateFlight(this, nextHop, Info.savedRoutes[$"{Name}-{nextHop.Name}"], objAirplane);
                createdFlights[objAirplane] = newFlight;

            }

            AssignTravellersToNextFlightOfAirplane(newFlight, objAirplane, nextHop, airport);

        }

        foreach (Flight tempFlight in createdFlights.Values)
        {

            tempFlight.StartFlight();

        }
    }

    public void LaunchNewPlaneAfterLanding(Flight flight)
    {
        Airplane airplane = flight.airplane;
        Flight newFlight = Auxiliary.CreateFlight(this, Info.GetLandingAirportOfAirplane(airplane), Info.GetRouteOfAirplane(airplane), airplane);

        newFlight.BoardFlight(TravellersToAirport);
        Instantiate(newFlight);
    }

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