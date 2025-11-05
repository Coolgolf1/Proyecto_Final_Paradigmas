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
        // Save data of airports 
        foreach (string city in Info.locations.Keys)
        {
            GameObject airportGO = Instantiate(airportPrefab, earth.transform);
            airportGO.name = city;

            Airport airport = airportGO.GetComponent<Airport>();
            airport.Name = city;
            Info.savedAirports[city] = airport;
            airport.id = Info.stringCityCodes[city];

            Location location = airportGO.GetComponentInChildren<Location>();
            location.Id = city;
            location.coords = Info.locations[city];
            location.Name = $"{city}_Loc";
        }

        // Save data of airplanes
        Airplane airplane = Instantiate(largeAirplanePrefab, earth.transform).GetComponent<AirplaneLarge>();
        Info.airplanes.Add(airplane);

        Airplane airplane2 = Instantiate(largeAirplanePrefab, earth.transform).GetComponent<AirplaneLarge>();
        Info.airplanes.Add(airplane2);

        // Save data of routes
        foreach (Tuple<string, string> routeTuple in Info.stringCityRoutes)
        {
            GameObject routeGO = Instantiate(routePrefab, earth.transform);
            routeGO.name = $"{routeTuple.Item1}-{routeTuple.Item2}";
            Route route = routeGO.GetComponent<Route>();
            route.airport1 = Info.savedAirports[routeTuple.Item1];
            route.airport2 = Info.savedAirports[routeTuple.Item2];
            //route.AddPlaneToRoute(airplane);
            //route.airport1.hangar.Add(airplane);
            Info.savedRoutes[routeGO.name] = route;
            Info.savedRoutes[$"{routeTuple.Item2}-{routeTuple.Item1}"] = route;
        }

        Info.savedAirports["Madrid"].hangar.Add(airplane);
        Info.savedAirports["Madrid"].hangar.Add(airplane2);


        // Init travellers in each airport
        foreach (Airport airport in Info.savedAirports.Values)
        {
            airport.InitTravellers();
        }

        // Load the real distances from dataset
        Auxiliary.LoadDistances(Info.savedRoutes);

        // Calculate initial Dijkstra Graph
        Info.CalculateDijkstraGraph();

        // Create Madrid-Dubai Flight
        //Flight flight = Auxiliary.CreateFlight(Info.savedAirports["Madrid"], Info.savedAirports["Dubai"], Info.savedRoutes["Madrid-Dubai"], airplane);

        List<Airport> madridDestinations = Info.savedAirports["Madrid"].TravellersToAirport.Keys.ToList();

        Dictionary<Airplane, Flight> madridFlights = new Dictionary<Airplane, Flight>();

        // For all travellers in origin airport, assign each of the travellers an airplane

        foreach (Airport airport in madridDestinations)
        {
            Flight flight;

            (Airplane objAirplane, Airport nextHop) = Info.savedAirports["Madrid"].FindAirplaneForTravellersToAirport(airport);

            if (objAirplane is null || nextHop is null)
            {
                continue;
            }

            if (madridFlights.Keys.Contains(objAirplane))
            {
                flight = madridFlights[objAirplane];
            }
            else
            {
                flight = Auxiliary.CreateFlight(Info.savedAirports["Madrid"], nextHop, Info.savedRoutes[$"Madrid-{nextHop.Name}"], objAirplane);
                madridFlights[objAirplane] = flight;
            }

            Info.savedAirports["Madrid"].AssignTravellersToNextFlightOfAirplane(flight, objAirplane, nextHop, airport);
        }

        
        foreach (Flight flight in madridFlights.Values)
        {
            
            flight.StartFlight();
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}