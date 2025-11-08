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
    public bool landed;

    public bool finished;

    public double FlightProgress { get; private set; }
    public double ElapsedKM { get; private set; }

    public Dictionary<Airport, int> TravellersToAirport { get; private set; }

    public void Awake()
    {
        TravellersToAirport = new Dictionary<Airport, int>();

        foreach (Airport airport in Info.savedAirports.Values)
        {
            TravellersToAirport.Add(airport, 0);
        }

        started = false;
        landed = false;
        finished = false;
    }

    //public void BoardFlight(Dictionary<Airport, int> passengers)
    //{
    //    int numPassengers = 0;
    //    foreach (Airport airport in Info.savedAirports.Values)
    //    {
    //        if (airport == airportOrig)
    //        {
    //            continue;
    //        }

    //        TravellersToAirport[airport] += passengers[airport];
    //        numPassengers += TravellersToAirport[airport];
    //    }

    //    if (numPassengers > airplane.Capacity)
    //    {
    //        throw new Exception("Airplane capacity surpassed.");
    //    }
    //}

    public void StartFlight()
    {
        started = true;
        FlightProgress = 0;
        ElapsedKM = 0;

        //foreach (Airport airport in Info.savedAirports.Values)
        //{
        //    if (airport != airportOrig)
        //    {
        //        airportOrig.TravellersToAirport[airport] -= TravellersToAirport[airport];
        //    }
        //}

        airportOrig.hangar.Remove(airplane);

        airportDest.TrackFlight(this);

    }

    public void EndFlight()
    {
        foreach (Airport airport in Info.savedAirports.Values)
        {
            if (airport != airportDest)
            {
                airportDest.TravellersToAirport[airport] += TravellersToAirport[airport];
                TravellersToAirport[airport] = 0;
            }
            else
            {
                TravellersToAirport[airport] = 0;
                // GIVE COINS FOR EACH PASSENGER TAKEN TO CORRECT AIRPORT SUCCESSFULLY ======================================
            }
        }
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

            List<Vector3> airplanePoints = new List<Vector3>(route.routePoints);

            if (route.airport1 == airportDest)
            {
                airplanePoints.Reverse();
            }


            (ElapsedKM, FlightProgress) = airplane.UpdatePosition(airplanePoints, route.distance, ElapsedKM);
            landed = CheckLanded(route.distance);
        }
        else if (landed && !finished)
        {
            // Remove plane from world simulation
            airplane.gameObject.SetActive(false);

            // Add Passengers to Airport
            EndFlight();

            finished = true;
            // Notify Airport of Landing
            OnLanded();


        }

        if (finished)
        {
            Destroy(this.gameObject);
        }
    }
}