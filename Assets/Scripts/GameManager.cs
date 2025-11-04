using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject airportPrefab;

    [SerializeField] private GameObject largeAirplanePrefab;

    [SerializeField]
    private GameObject routePrefab;

    [SerializeField]
    private GameObject earth;

    private Dictionary<string, Airport> savedAirports = new Dictionary<string, Airport>();
    private Dictionary<string, Route> savedRoutes = new Dictionary<string, Route>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        foreach (string city in Auxiliary.locations.Keys)
        {
            var airportGO = Instantiate(airportPrefab, earth.transform);
            airportGO.name = city;

            Airport airport = airportGO.GetComponent<Airport>();
            airport.Name = city;
            savedAirports[city] = airport;
            airport.id = Auxiliary.codes[city];

            Location locationComp = airportGO.GetComponentInChildren<Location>();
            locationComp.Id = city;
            locationComp.coords = Auxiliary.locations[city];
            locationComp.Name = $"{city}_Loc";
        }

        foreach (Tuple<string, string> routeTuple in Auxiliary.availableRoutes)
        {
            var route = Instantiate(routePrefab, earth.transform);
            Route rutaComp = route.GetComponent<Route>();
            rutaComp.airport1 = savedAirports[routeTuple.Item1];
            rutaComp.airport2 = savedAirports[routeTuple.Item2];
            route.name = $"{routeTuple.Item1}-{routeTuple.Item2}";
            savedRoutes[route.name] = rutaComp;
        }

        GameObject flightTest = new GameObject();
        flightTest.name = "TestRuta";
        Flight flightComp = flightTest.AddComponent<Flight>();
        flightComp.route = savedRoutes["Madrid-Dubai"];
        flightComp.airplane = Instantiate(largeAirplanePrefab, earth.transform).GetComponent<AirplaneLarge>();

        flightComp.route.distance = 400;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}