using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class Score
{
    private static int _score = 0;

    private static string _path = Path.Combine(Application.streamingAssetsPath, "../highscore.txt");

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
        string highScoreString = File.ReadAllText(_path);
        int highScore = int.Parse(highScoreString);
        return highScore;
    }

    public static void SaveHighScore()
    {
        byte[] bytes = Encoding.UTF8.GetBytes(_score.ToString());

        using FileStream file = new FileStream(_path, FileMode.Create, FileAccess.Write);
        file.Write(bytes, 0, bytes.Length);
    }
}
