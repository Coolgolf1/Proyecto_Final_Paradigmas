
public class EconomyManager
{
    private static EconomyManager _instance;

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
        Player.Money += passengers;
    }
}
