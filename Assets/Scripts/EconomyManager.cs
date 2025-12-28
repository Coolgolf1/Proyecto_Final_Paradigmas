
using System;

public class EconomyManager
{
    private static EconomyManager _instance;

    public event EventHandler MoneyChange;

    public bool mainMenuGame;

    public EconomyManager() { }

    public static EconomyManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new EconomyManager();
        }

        return _instance;
    }

    public void SaveCoins(int passengers, double distance)
    {
        if (mainMenuGame)
            return;

        if (passengers > 0)
        {
            Player.Money += (long)(passengers * distance / 25);
            Player.UpdateScore(passengers);
            MoneyChange?.Invoke(this, EventArgs.Empty);
        }
    }

    public void SetCoins(long money)
    {
        Player.Money = money;
        MoneyChange?.Invoke(this, EventArgs.Empty);
    }

    public bool SubtractCoins(long money)
    {
        if (Player.Money < money)
        {
            return false;
        }
        Player.Money -= money;
        MoneyChange?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool AddCoins(long money)
    {
        Player.Money += money;
        MoneyChange?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public long GetBalance()
    {
        return Player.Money;
    }
}
