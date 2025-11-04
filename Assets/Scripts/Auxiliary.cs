using System.Collections.Generic;
using System.IO;
using UnityEngine;

static class Auxiliary
{
    static string pathName = Path.Combine(Application.streamingAssetsPath, "flight_distance.csv");


    public static Dictionary<string, string> codes = new Dictionary<string, string>()
    {
        { "Madrid", "mad" },
        { "San Francisco", "sfo" },
        { "Shanghai", "pvg" },
        { "Paris", "cdg" },
        { "Dubai", "dxb" }
    };

    public static void LoadDistances(Dictionary<string, Route> routes)
    {
        string[] lines = File.ReadAllLines(pathName)[1..];

        foreach (string routeName in routes.Keys)
        {
            Route route = routes[routeName];

            string a1 = route.airport1.Name;
            string a2 = route.airport2.Name;

            string codeA1 = codes[a1];
            string codeA2 = codes[a2];

            foreach (string line in lines)
            {
                string[] data = line.Split(",");
                string routeCodes = data[0];

                if (routeCodes == $"{codeA1}-{codeA2}" || routeCodes == $"{codeA2}-{codeA1}")
                {
                    double distance = double.Parse(data[1]);
                    route.distance = distance;
                    break;
                }
            }
        }

    }
}
