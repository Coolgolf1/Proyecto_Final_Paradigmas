using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Airport : MonoBehaviour
{
    public string id;
    public string Name;
    public int numberOfRunways;

    public List<Airplane> hangar = new List<Airplane>();

    public Dictionary<Airport, int> TravellersToAirport;

    [SerializeField]
    public Location location;
    public GameObject modelPrefab;

    public void InitNumberOfTravellersToAirports()
    {
        if (TravellersToAirport == null)
        {
            TravellersToAirport = new Dictionary<Airport, int>();

            foreach (Airport airport in Info.savedAirports.Values.ToList())
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

    public Airplane FindFlightForTravellersToAirport(Airport objectiveAirport)
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
        GameObject flightGO = new GameObject();
        Flight flight = flightGO.AddComponent<Flight>();
        flight.airplane = objAirplane;
        flight.airportOrig = this;
        flight.airportDest = Info.GetLandingAirportOfAirplane(objAirplane);
        flight.route = Info.GetRouteOfAirplane(objAirplane);

        int occupiedCapacity = flight.TravellersToAirport.Values.ToList().Sum();
        int remainingCapacity = objAirplane.Capacity - occupiedCapacity;

        int travellersInAirport = TravellersToAirport[objAirport];

        if (travellersInAirport <= remainingCapacity)
        {
            flight.TravellersToAirport[objAirport] += travellersInAirport;
        } 
        else
        {
            flight.TravellersToAirport[objAirport] += (travellersInAirport - remainingCapacity);
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
            Airplane objAirplane = FindFlightForTravellersToAirport(airport);
            AssignTravellersToNextFlightOfAirplane(objAirplane, airport);
        }


        // Create new flight from flight that landed
        LaunchNewPlaneAfterLanding(flight);
    }

    public void LaunchNewPlaneAfterLanding(Flight flight)
    {
        Airplane landedAirplane = flight.airplane;
        Flight newFlight = new Flight();
        newFlight.airplane = landedAirplane;
        newFlight.airportOrig = this;
        newFlight.airportDest = Info.GetLandingAirportOfAirplane(landedAirplane);
        newFlight.route = Info.GetRouteOfAirplane(landedAirplane);

        newFlight.BoardFlight(TravellersToAirport);
        Instantiate(newFlight);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        transform.position = location.coords;

        Vector3 up = (location.coords - Vector3.zero).normalized;

        this.transform.rotation = Quaternion.LookRotation(Vector3.forward, up);

        InitNumberOfTravellersToAirports();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}