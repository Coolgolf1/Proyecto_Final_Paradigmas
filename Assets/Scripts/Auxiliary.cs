using System.Collections.Generic;
using System.IO;
using UnityEngine;

static class Auxiliary
{
    static string pathName = Path.Combine(Application.streamingAssetsPath, "../flight_distance.csv");

    static double defaultDistance = 10000;

    public static void LoadDistances(Dictionary<string, Route> routes)
    {
        string[] lines = File.ReadAllLines(pathName)[1..];

        foreach (string routeName in routes.Keys)
        {
            Route route = routes[routeName];

            string a1 = route.airport1.Name;
            string a2 = route.airport2.Name;

            string codeA1 = Info.stringCityCodes[a1];
            string codeA2 = Info.stringCityCodes[a2];

            bool found = false;
            foreach (string line in lines)
            {
                string[] data = line.Split(",");
                string routeCodes = data[0];

                if (routeCodes == $"{codeA1}-{codeA2}" || routeCodes == $"{codeA2}-{codeA1}")
                {
                    double distance = double.Parse(data[1]);
                    route.distance = distance;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                route.distance = defaultDistance;

                // REMOVE DEBUG AFTER MAKING SURE THIS WORKS 100%!!! =================================
                Debug.Log($"ERROR LOADING DISTANCE FOR ROUTE: {route.name}.");
            }
        }
    }

    public static Flight CreateFlight(Airport airportOrig, Airport airportDest, Route route, Airplane airplane)
    {

        GameObject flightGO = new GameObject();
        flightGO.name = $"{airportOrig.Name}-{airportDest.Name}";
        Flight flight = flightGO.AddComponent<Flight>();

        flight.airplane = airplane;
        flight.airportOrig = airportOrig;
        flight.airportDest = airportDest;
        flight.route = route;
        Info.flights.Add(flight);

        return flight;
    }
}
