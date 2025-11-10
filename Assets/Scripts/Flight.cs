using System;
using System.Collections.Generic;
using UnityEngine;

public class Flight : MonoBehaviour
{
    public Airport airportOrig;
    public Airport airportDest;

    public Route route;
    public Airplane airplane;

    public bool started;

    public event EventHandler LandedEvent;
    public event EventHandler TakeOffEvent;
    public bool landed;

    public bool finished;

    public double FlightProgress { get; private set; }
    public double ElapsedKM { get; private set; }

    public Dictionary<Airport, int> TravellersToAirport { get; private set; }

    private InfoSingleton _info = InfoSingleton.GetInstance();

    public string flightID;

    public void Awake()
    {
        TravellersToAirport = new Dictionary<Airport, int>();

        foreach (Airport airport in _info.savedAirports.Values)
        {
            TravellersToAirport.Add(airport, 0);
        }

        started = false;
        landed = false;
        finished = false;
    }

    public void StartFlight()
    {
        airportOrig.TrackTakeOff(this);
        airportDest.TrackFlight(this);

        started = true;
        FlightProgress = 0;
        ElapsedKM = 0;

        //foreach (Airport airport in info.savedAirports.Values)
        //{
        //    if (airport != airportOrig)
        //    {
        //        airportOrig.TravellersToAirport[airport] -= TravellersToAirport[airport];
        //    }
        //}

        OnTakeOff();
    }

    public void EndFlight()
    {
        //foreach (Airport airport in _info.savedAirports.Values)
        //{
        //    TravellersToAirport[airport] = 0;
        //}

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
        if (started && !landed)
        {
            airplane.gameObject.SetActive(true);

            List<Vector3> airplanePoints = new List<Vector3>(route.RoutePoints);

            if (route.Airport1 == airportDest)
            {
                airplanePoints.Reverse();
            }


            (ElapsedKM, FlightProgress) = airplane.UpdatePosition(airplanePoints, route.Distance, ElapsedKM);
            landed = CheckLanded(route.Distance);
        }
        else if (landed && !finished)
        {
            // Remove plane from world simulation
            airplane.gameObject.SetActive(false);

            // End flight
            EndFlight();

            // Notify Airport of Landing
            OnLanded();

            finished = true;
        }

        if (finished)
        {
            Destroy(this.gameObject);
        }
    }
}