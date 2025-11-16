
using System;

public class EconomyManager
{
    private static EconomyManager _instance;

    public event EventHandler MoneyChange;

    public EconomyManager() { }

    public static EconomyManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new EconomyManager();
        }

        return _instance;
    }

    public void SaveCoins(int passengers)
    {
        if (passengers > 0)
        {
            Player.Money += passengers;
            MoneyChange?.Invoke(this, EventArgs.Empty);
        }
    }
}
