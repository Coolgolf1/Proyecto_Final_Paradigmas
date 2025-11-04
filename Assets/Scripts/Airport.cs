using System;
using System.Collections.Generic;
using UnityEngine;

public class Airport : MonoBehaviour
{
    public string Id;
    public string Name;
    public int NumberOfRunways;

    public Dictionary<Airport, int> TravellersToAirport { get; private set; }

    [SerializeField]
    public Location location;
    public GameObject modelPrefab;

    public void InitNumberOfTravellersToAirports()
    {
        foreach (Airport airport in AirportList.items)
        {
            if (airport != this)
            {
                // TODO: CHANGE LATER TO RANDOM OR SOMETHING DIFFERENT ======================================
                TravellersToAirport[airport] = 10;
            }
        }
    }

    public void TrackFlight(Flight flight)
    {
        flight.LandedEvent += HandleLanding;
    }

    public void HandleLanding(object sender, EventArgs e)
    {
        Flight flight = (Flight)sender;

        // UPDATE DIJKSTRA IN AIRPORT FOR NEW TRAVELLERS

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        transform.position = location.coords;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}