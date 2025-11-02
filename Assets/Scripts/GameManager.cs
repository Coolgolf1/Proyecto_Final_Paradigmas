using UnityEngine;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private GameObject airportPrefab;
    [SerializeField] private GameObject largeAirplanePrefab;
    [SerializeField]
    private GameObject routePrefab;
    [SerializeField]
    private GameObject earth;

    private Dictionary<string, Vector3> locations = new Dictionary<string, Vector3>()
    {
        { "Madrid", new Vector3(11.2700005f, 14.1899996f, -8.56000042f) },
        { "San Francisco", new Vector3(-14.2299995f,14.0200005f,-2.3499999f) },
        { "Shanghai", new Vector3(3.6400001f,7.21999979f,18.3999996f) },
        { "Paris", new Vector3(11.1300001f,15.6400003f,-5.75f ) },
        { "Dubai", new Vector3(18.3099995f,6.26000023f,5.48000002f) }
    };

    private List<Tuple<string, string>> availableRoutes = new List<Tuple<string, string>>()
    {
        new Tuple<string, string>("Madrid", "Dubai"),
        new Tuple<string, string>("Madrid", "Paris"),
        new Tuple<string, string>("Paris", "San Francisco"),
        new Tuple<string, string>("Dubai", "Shanghai"),
        new Tuple<string, string>("Paris", "Shanghai"),
        new Tuple<string, string>("San Francisco", "Shanghai")
    };

    private Dictionary<string, Airport> savedAirports = new Dictionary<string, Airport>();
    private Dictionary<string, Route> savedRoutes = new Dictionary<string, Route>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (string city in locations.Keys)
        {
            var airportGO = Instantiate(airportPrefab, earth.transform);
            airportGO.name = city;

            Airport airport = airportGO.GetComponent<Airport>();
            airport.name = $"{city}";
            savedAirports[city] = airport;


            Location locationComp = airportGO.GetComponentInChildren<Location>();
            locationComp.id = city;
            locationComp.coords = locations[city];
            locationComp.name = $"{city}_Loc";

        }

        foreach (Tuple<string, string> routeTuple in availableRoutes)
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

        flightComp.distance = 400;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
