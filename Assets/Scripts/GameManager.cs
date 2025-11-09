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

    [SerializeField] private AirportUI airportUI;
    [SerializeField] private Camera playerCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Info.airportUI = airportUI;
        Info.playerCamera = playerCamera;

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

            Info.airplanesGoingFromEmptyAirport[airport] = new List<Airplane>();
        }

        //Info.savedAirports["Madrid"].TravellersToAirport[Info.savedAirports["Paris"]] = 10;
        Info.savedAirports["Madrid"].TravellersToAirport[Info.savedAirports["Dubai"]] = 80;
        Info.savedAirports["Madrid"].TravellersToAirport[Info.savedAirports["San Francisco"]] = 10;
        Info.savedAirports["Madrid"].TravellersToAirport[Info.savedAirports["Shanghai"]] = 120;
        //Info.savedAirports["Madrid"].TravellersToAirport[Info.savedAirports["Paris"]] = 10;

        // Load the real distances from dataset
        Auxiliary.LoadRouteDistances(Info.savedRoutes);

        // Calculate initial Dijkstra Graph
        Info.CalculateDijkstraGraph();

        // Create Madrid-Dubai Flight
        //Flight flight = Auxiliary.CreateFlight(Info.savedAirports["Madrid"], Info.savedAirports["Dubai"], Info.savedRoutes["Madrid-Dubai"], airplane);

        List<Airport> destinations = Info.savedAirports.Values.ToList();

        Dictionary<Airplane, Flight> madridFlights = new Dictionary<Airplane, Flight>();

        // For all travellers in origin airport, assign each of the travellers an airplane

        Queue<Airport> airportQueue = new Queue<Airport>(destinations);

        while (airportQueue.Count > 0)
        {
            Airport airport = airportQueue.Dequeue();

            if (airport == Info.savedAirports["Madrid"])
            {
                continue;
            }

            if (Info.savedAirports["Madrid"].TravellersToAirport[airport] <= 0)
            {
                continue;
            }

            HashSet<Airplane> usedThisIteration = new HashSet<Airplane>();

            while (Info.savedAirports["Madrid"].TravellersToAirport[airport] > 0)
            {
                Flight flight;

                (Airplane objAirplane, Airport nextHop) = Info.savedAirports["Madrid"].FindAirplaneForTravellersToAirport(airport);

                if (objAirplane is null || nextHop is null)
                {
                    break;
                }

                if (usedThisIteration.Contains(objAirplane))
                {
                    break; // no more airplanes left
                }
                usedThisIteration.Add(objAirplane);


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

            //if (Info.savedAirports["Madrid"].TravellersToAirport[airport] > 0)
            //{
            //    airportQueue.Enqueue(airport);
            //}
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