using System.Collections.Generic;
using System.IO;
using UnityEngine;

static class Auxiliary
{
    private static string _pathName = Path.Combine(Application.streamingAssetsPath, "../flight_distance.csv");

    private static double _defaultDistance = 10000;

    private static InfoSingleton _info = InfoSingleton.GetInstance();

    public static double GetDistanceBetweenAirports(Airport airport1, Airport airport2)
    {
        if (airport1 == airport2)
        {
            return 0;
        }

        string[] lines = File.ReadAllLines(_pathName)[1..];

        string a1 = airport1.Name;
        string a2 = airport2.Name;

        string codeA1 = _info.stringCityCodes[a1];
        string codeA2 = _info.stringCityCodes[a2];

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
        return _defaultDistance;
    }

    public static void LoadRouteDistances(Dictionary<string, Route> routes)
    {
        string[] lines = File.ReadAllLines(_pathName)[1..];

        foreach (string routeName in routes.Keys)
        {
            Route route = routes[routeName];

            string a1 = route.Airport1.Name;
            string a2 = route.Airport2.Name;

            string codeA1 = _info.stringCityCodes[a1];
            string codeA2 = _info.stringCityCodes[a2];

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
                route.SetDistance(_defaultDistance);

                // REMOVE DEBUG AFTER MAKING SURE THIS WORKS 100%!!! =================================
                Debug.Log($"ERROR LOADING DISTANCE FOR ROUTE: {route.name}.");
            }
        }
    }
}
