using System;
using System.Collections.Generic;
using System.Linq;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        foreach (string city in Info.locations.Keys)
        {
            var airportGO = Instantiate(airportPrefab, earth.transform);
            airportGO.name = city;

            Airport airport = airportGO.GetComponent<Airport>();
            airport.Name = city;
            Info.savedAirports[city] = airport;
            airport.id = Info.codes[city];

            Location locationComp = airportGO.GetComponentInChildren<Location>();
            locationComp.Id = city;
            locationComp.coords = Info.locations[city];
            locationComp.Name = $"{city}_Loc";
        }

        foreach (Tuple<string, string> routeTuple in Info.availableRoutes)
        {
            var route = Instantiate(routePrefab, earth.transform);
            Route rutaComp = route.GetComponent<Route>();
            rutaComp.airport1 = Info.savedAirports[routeTuple.Item1];
            rutaComp.airport2 = Info.savedAirports[routeTuple.Item2];
            route.name = $"{routeTuple.Item1}-{routeTuple.Item2}";
            Info.savedRoutes[route.name] = rutaComp;
        }

        Auxiliary.LoadDistances(Info.savedRoutes);

        GameObject flightTest = new GameObject();
        flightTest.name = "Madrid-Dubai";
        Flight flightComp = flightTest.AddComponent<Flight>();
        flightComp.route = Info.savedRoutes["Madrid-Dubai"];
        flightComp.airplane = Instantiate(largeAirplanePrefab, earth.transform).GetComponent<AirplaneLarge>();

        flightComp.route.distance = 400;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}