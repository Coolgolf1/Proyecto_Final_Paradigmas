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

        foreach (Airport airport in Info.savedAirports.Values.ToList())
        {
            if (airport != this)
            {
                // TODO: CHANGE LATER TO RANDOM OR SOMETHING DIFFERENT ======================================
                TravellersToAirport[airport] = 10;
            }
        }
    }

    public Airplane FindAirplaneForTravellersToAirport(Airport objectiveAirport)
    {
        Airplane nextAirplane = RouteAssigner.Dijkstra(Info.DijkstraGraph, this, objectiveAirport);

        if (nextAirplane is null)
        {
            Debug.Log("Cannot find next airplane of path for travellers.");
            return null;
        }

        return nextAirplane;
    }

    public void TrackFlight(Flight flight)
    {
        flight.LandedEvent += HandleLanding;
    }

    public void AssignTravellersToNextFlightOfAirplane(Airplane objAirplane, Airport objAirport)
    {
        // Create New Flight of Airplane
        Flight flight = Auxiliary.CreateFlight(this, objAirport, Info.GetRouteOfAirplane(objAirplane), objAirplane);

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

        // UPDATE DIJKSTRA IN AIRPORT FOR NEW TRAVELLERS ======================================
        Info.CalculateDijkstraGraph();
        foreach (Airport airport in TravellersToAirport.Keys)
        {
            Airplane objAirplane = FindAirplaneForTravellersToAirport(airport);
            AssignTravellersToNextFlightOfAirplane(objAirplane, airport);
        }

        // Create new flight from flight that landed
        LaunchNewPlaneAfterLanding(flight);
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

        Vector3 up = (location.coords - Vector3.zero).normalized;

        this.transform.rotation = Quaternion.LookRotation(Vector3.forward, up);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}