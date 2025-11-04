using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

static class Auxiliary
{
    static string pathName = Path.Combine(Application.streamingAssetsPath, "flight_distance.csv");

    public static Dictionary<string, Vector3> locations = new Dictionary<string, Vector3>()
    {
        { "Madrid", new Vector3(11.2700005f, 14.1899996f, -8.56000042f) },
        { "San Francisco", new Vector3(-14.2299995f,14.0200005f,-2.3499999f) },
        { "Shanghai", new Vector3(3.6400001f,7.21999979f,18.3999996f) },
        { "Paris", new Vector3(11.1300001f,15.6400003f,-5.75f ) },
        { "Dubai", new Vector3(18.3099995f,6.26000023f,5.48000002f) }
    };

    public static List<Tuple<string, string>> availableRoutes = new List<Tuple<string, string>>()
    {
        new Tuple<string, string>("Madrid", "Dubai"),
        new Tuple<string, string>("Madrid", "Paris"),
        new Tuple<string, string>("Paris", "San Francisco"),
        new Tuple<string, string>("Dubai", "Shanghai"),
        new Tuple<string, string>("Paris", "Shanghai"),
        new Tuple<string, string>("San Francisco", "Shanghai")
    };

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

    public static Airport GetCurrentAirportOfAirplane(Airplane airplane)
    {
        // FIX THIS TO USE THINGS WE HAVE NOT STATIC CLASS
        foreach (Airport airport in AirportList.items)
        {
            if (airport.hangar.Contains(airplane))
            {
                return airport;
            }
        }

        return null;
    }
}
