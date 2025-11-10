using System.Collections.Generic;
using System.IO;
using UnityEngine;

static class Auxiliary
{
    static string pathName = Path.Combine(Application.streamingAssetsPath, "../flight_distance.csv");

    static double defaultDistance = 10000;

    private static InfoSingleton info = InfoSingleton.GetInstance();


    public static double GetDistanceBetweenAirports(Airport airport1, Airport airport2)
    {
        if (airport1 == airport2)
        {
            return 0;
        }

        string[] lines = File.ReadAllLines(pathName)[1..];

        string a1 = airport1.Name;
        string a2 = airport2.Name;

        string codeA1 = info.stringCityCodes[a1];
        string codeA2 = info.stringCityCodes[a2];

        foreach (string line in lines)
        {
            string[] data = line.Split(",");
            string codes = data[0];

            if (codes == $"{codeA1}-{codeA2}" || codes == $"{codeA2}-{codeA1}")
            {
                double distance = double.Parse(data[1]);
                return distance;
            }
        }

        // If search fails
        Debug.Log($"DISTANCE FAIL SEARCH {airport1}-{airport2}.");
        return defaultDistance;
    }

    public static void LoadRouteDistances(Dictionary<string, Route> routes)
    {
        string[] lines = File.ReadAllLines(pathName)[1..];

        foreach (string routeName in routes.Keys)
        {
            Route route = routes[routeName];

            string a1 = route.Airport1.Name;
            string a2 = route.Airport2.Name;

            string codeA1 = info.stringCityCodes[a1];
            string codeA2 = info.stringCityCodes[a2];

            bool found = false;
            foreach (string line in lines)
            {
                string[] data = line.Split(",");
                string routeCodes = data[0];

                if (routeCodes == $"{codeA1}-{codeA2}" || routeCodes == $"{codeA2}-{codeA1}")
                {
                    double distance = double.Parse(data[1]);
                    route.SetDistance(distance);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                route.SetDistance(defaultDistance);

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
        info.flights.Add(flight);

        return flight;
    }
}
