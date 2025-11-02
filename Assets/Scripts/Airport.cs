using Assets.Scripts;
using UnityEngine;
using System.Collections.Generic;

public class Airport : MonoBehaviour
{
    public string Id { get; private set; }
    public string Name { get; private set; }

    public int NumberOfRunways { get; private set; }

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = location.coords;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
