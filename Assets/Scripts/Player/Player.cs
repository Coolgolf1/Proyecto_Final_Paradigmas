using System;
using System.Collections.Generic;
using UnityEngine;


public static class Player
{
    public static int Score { get; set; } = 0;
    public static long Money { get; set; } = 0;
    public static List<Airplane> Airplanes { get; private set; }
    public static List<Airport> UnlockedAirports { get; set; } = new List<Airport>();

    private const string _scoreName = "highScore";

    public static void UpdateScore(int coins)
    {
        Score += (int)Math.Ceiling(coins * 0.01);
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt(_scoreName);
    }

    public static void SaveHighScore()
    {
        PlayerPrefs.SetInt(_scoreName, Score);
        PlayerPrefs.Save();
    }

    public static void UnlockAirport(Airport airport)
    {
        UnlockedAirports.Add(airport);
        airport.Unlock();
        
    }

    public static void Restart()
    {
        Score = 0;
        //Money = 1000000;
        UnlockedAirports.Clear();
    }
}