using System.Collections.Generic;
using UnityEngine;

public class Flight : MonoBehaviour
{
    public Airport AirportOrig;
    public Airport AirportDest;

    public Route route;
    public Airplane airplane;
    

    public double distance;
    private bool started;

    public Dictionary<Airport, int> TravellersToAirport { get; private set; }

    public void StartFlight()
    {
        foreach (Airport airport in AirportList.items)
        {
            if (airport != AirportOrig)
            {
                AirportOrig.TravellersToAirport[airport] -= TravellersToAirport[airport];
            }

            started = true;
        }
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

            started = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        started = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            airplane.gameObject.SetActive(true);
            started = airplane.UpdatePosition(route.routePoints, distance);
        }
        else
        {
            airplane.gameObject.SetActive(false);
        }
    }
}
