using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Flight : MonoBehaviour
{
    public Airport AirportOrig { get; private set; }
    public Airport AirportDest { get; private set; }
    public Route Route { get; private set; }
    public Airplane Airplane { get; private set; }
    public string FlightID { get; private set; }
    public bool Started { get; private set; }
    public bool Landed { get; private set; }
    public bool Finished { get; private set; }

    public event EventHandler LandedEvent;
    public event EventHandler TakeOffEvent;

    private InfoSingleton _info = InfoSingleton.GetInstance();

    public double FlightProgress { get; private set; }
    public double ElapsedKM { get; private set; }

    public Dictionary<Airport, int> TravellersToAirport { get; private set; }

    public void Initialise(Airport airportOrig, Airport airportDest, Route route, Airplane airplane)
    {
        AirportOrig = airportOrig;
        AirportDest = airportDest;
        Route = route;
        Airplane = airplane;

        string codeOrig = _info.stringCityCodes[route.Airport1.Name].ToUpper();
        string codeDest = _info.stringCityCodes[route.Airport2.Name].ToUpper();

        FlightID = $"{codeOrig[0]}{codeDest[0]}{route.NumberOfFlightsOnRoute:D4}";

        Route.AddFlightToRoute();
    }

    public void Awake()
    {
        TravellersToAirport = new Dictionary<Airport, int>();

        foreach (Airport airport in _info.savedAirports.Values)
        {
            TravellersToAirport.Add(airport, 0);
        }

        Started = false;
        Landed = false;
        Finished = false;
    }

    public void StartFlight()
    {
        AirportOrig.TrackTakeOff(this);
        AirportDest.TrackLanding(this);

        Started = true;
        FlightProgress = 0;
        ElapsedKM = 0;

        OnTakeOff();
    }

    public void Embark(int passengers, Airport objAirport)
    {
        int occupiedCapacity = TravellersToAirport.Values.ToList().Sum();

        int remainingCapacity = Airplane.Capacity - occupiedCapacity;

        int travellersInAirport = AirportOrig.TravellersToAirport[objAirport];

        if (travellersInAirport <= remainingCapacity)
        {
            TravellersToAirport[objAirport] += travellersInAirport;
            AirportOrig.TravellersToAirport[objAirport] -= TravellersToAirport[objAirport];
        }
        else
        {
            TravellersToAirport[objAirport] += remainingCapacity;
            AirportOrig.TravellersToAirport[objAirport] -= remainingCapacity;
        }
    }

    public void Disembark()
    {
        foreach (Airport airport in _info.savedAirports.Values)
        {
            AirportDest.ReceivePassengers(TravellersToAirport[airport], airport);
            TravellersToAirport[airport] = 0;
        }
    }

    public void EndFlight()
    {
        _info.flights.Remove(this);
    }

    public bool CheckLanded(double totalDistance)
    {
        if (FlightProgress < 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    protected virtual void OnLanded()
    {

        LandedEvent?.Invoke(this, EventArgs.Empty);

    }
    protected virtual void OnTakeOff()
    {

        TakeOffEvent?.Invoke(this, EventArgs.Empty);

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //private void Start()
    //{

    //    started = false;
    //    landed = false;
    //}

    // Update is called once per frame
    private void Update()
    {
        if (Started && !Landed)
        {
            Airplane.gameObject.SetActive(true);

            List<Vector3> airplanePoints = new List<Vector3>(Route.RoutePoints);

            if (Route.Airport1 == AirportDest)
            {
                airplanePoints.Reverse();
            }


            (ElapsedKM, FlightProgress) = Airplane.UpdatePosition(airplanePoints, Route.Distance, ElapsedKM);
            Landed = CheckLanded(Route.Distance);
        }
        else if (Landed && !Finished)
        {
            // Remove plane from world simulation
            Airplane.gameObject.SetActive(false);

            // Disembark and end flight
            Disembark();
            EndFlight();

            // Notify Airport of Landing
            OnLanded();

            Finished = true;
        }

        if (Finished)
        {
            Destroy(this.gameObject);
        }
    }
}