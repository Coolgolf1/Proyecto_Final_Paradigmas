using System;
using UnityEngine;

public static class Score
{
    private static int _score = 0;

    private const string _scoreName = "highScore";

    public static void UpdateScore(int coins)
    {
        _score += (int)Math.Floor(coins * 0.01);
    }

    public static int GetScore()
    {
        return _score;
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt(_scoreName);
    }

    public static void SaveHighScore()
    {
        PlayerPrefs.SetInt(_scoreName, _score);
        PlayerPrefs.Save();
    }
}
