using System.Collections.Generic;
using UnityEngine;

public class Flight : MonoBehaviour
{
    public Airport AirportOrig;
    public Airport AirportDest;
    public Airplane airplane;

    public double distance;

    public Dictionary<Airport, int> TravellersToAirport { get; private set; }

    public void StartFlight()
    {
        foreach (Airport airport in AirportList.items)
        {
            if (airport != AirportOrig)
            {
                AirportOrig.TravellersToAirport[airport] -= TravellersToAirport[airport];
            }
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
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
