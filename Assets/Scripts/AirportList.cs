using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public static class AirportList
{
    public static List<Airport> items {  get; private set; }

    public static void Init(List<Airport> airports)
    {
        foreach (Airport airport in airports) 
        { 
            items.Add(airport);
        }
    }
}
