using System;
using System.Collections.Generic;
using UnityEngine;

public class Flight : MonoBehaviour
{
    public Airport airportOrig;
    public Airport airportDest;

    public Route route;
    public Airplane airplane;

    private bool started;

    public event EventHandler LandedEvent;
    public bool landed;

    public bool finished = false;

    public double FlightProgress { get; private set; }
    public double ElapsedKM { get; private set; }
    public int targetIndex;
    public float indexProgress;

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
        FlightProgress = 0;
        ElapsedKM = 0;

        foreach (Airport airport in AirportList.items)
        {
            if (airport != airportOrig)
            {
                airportOrig.TravellersToAirport[airport] -= TravellersToAirport[airport];
            }
        }

        airportDest.TrackFlight(this);
    }

    public void EndFlight()
    {
        foreach (Airport airport in AirportList.items)
        {
            if (airport != airportDest && airport != airportOrig)
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

    public void UpdateAirplanePosition()
    {
        ElapsedKM += airplane.Speed * Time.deltaTime;
        FlightProgress = ElapsedKM / route.distance;

        if (FlightProgress < 1)
        {
            indexProgress = (float)(FlightProgress * (route.routePoints.Count - 1));

            targetIndex = (int)Mathf.Floor(indexProgress);

            airplane.transform.position = Vector3.Lerp(route.routePoints[targetIndex], route.routePoints[targetIndex + 1], indexProgress - targetIndex);
            airplane.transform.LookAt(route.routePoints[targetIndex + 1], this.transform.position - Vector3.zero);
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
    private void Start()
    {
        // BoardFlight();
        started = true;
        landed = false;

        // REMOVE THIS LATER AS IT IS IN STARTFLIGHT
        FlightProgress = 0;
        ElapsedKM = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (started && !landed)
        {
            // StartFlight();
            airplane.gameObject.SetActive(true);
            UpdateAirplanePosition();
            landed = CheckLanded(route.distance);
        }
        else if (landed)
        {
            // Remove plane from world simulation
            airplane.gameObject.SetActive(false);

            // Add Passengers to Airport
            // EndFlight();

            // Notify Airport of Landing
            // OnLanded();

            finished = true;
        }

        if (finished)
        {
            Destroy(this);
        }
    }
}