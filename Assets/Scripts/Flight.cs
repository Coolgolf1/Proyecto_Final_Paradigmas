using System;
using System.Collections.Generic;
using UnityEngine;

public class Flight : MonoBehaviour
{
    public Airport AirportOrig;
    public Airport AirportDest;

    public Route route;
    public Airplane airplane;

    private bool started;

    public event EventHandler LandedEvent;
    public bool landed;

    public bool finished = false;

    public Dictionary<Airport, int> TravellersToAirport { get; private set; }

    public void BoardFlight(Dictionary<Airport, int> passengers)
    {
        int numPassengers = 0;
        foreach (Airport airport in AirportList.items)
        {
            TravellersToAirport[airport] = passengers[airport];
            numPassengers += passengers[airport];
        }

        if (numPassengers > airplane.Capacity)
        {
            throw new Exception("Airplane capacity surpassed.");
        }
    }

    public void StartFlight()
    {
        foreach (Airport airport in AirportList.items)
        {
            if (airport != AirportOrig)
            {
                AirportOrig.TravellersToAirport[airport] -= TravellersToAirport[airport];
            }
        }

        AirportDest.TrackFlight(this);
    }

    public void EndFlight()
    {
        foreach (Airport airport in AirportList.items)
        {
            if (airport != AirportDest && airport != AirportOrig)
            {
                AirportDest.TravellersToAirport[airport] += TravellersToAirport[airport];
                TravellersToAirport[airport] = 0;
            }
            else
            {
                TravellersToAirport[airport] = 0;
                // GIVE COINS FOR EACH PASSENGER TAKEN TO CORRECT AIRPORT SUCCESSFULLY
            }
        }
    }

    protected virtual void OnLanded()
    {
        LandedEvent?.Invoke(this, EventArgs.Empty);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        started = true;
        landed = false;
        airplane.CurrentAirport = AirportOrig;
    }

    // Update is called once per frame
    private void Update()
    {
        if (started && !landed)
        {
            // StartFlight();
            airplane.gameObject.SetActive(true);
            airplane.UpdatePosition(route.routePoints, route.distance);
            landed = airplane.CheckLanded(route.distance);
        }
        else if (landed)
        {
            // Remove plane from world simulation
            airplane.gameObject.SetActive(false);

            // Update current Airport of airplane
            airplane.CurrentAirport = AirportDest;

            // Add Passengers to Airport
            // EndFlight();

            // Notify Airport of Landing
            //OnLanded();

            finished = true;
        }

        if (finished)
        {
            Destroy(this);
        }
    }
}