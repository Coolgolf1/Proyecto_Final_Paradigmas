using UnityEngine;

public class AirplaneFactory
{
    private static int _counter = 0;

    private static AirplaneFactory _instance;

    public static AirplaneSpawner _spawner;

    public static AirplaneFactory GetInstance()
    {
        if (_instance == null)
        {
            _instance = new AirplaneFactory();
        }
        return _instance;
    }

    public void Initialise(AirplaneSpawner spawner)
    {
        _spawner = spawner;
    }

    public Airplane BuildAirplane(AirplaneTypes type, Transform earthTransform)
    {
        Airplane airplane = _spawner.InstantiateAirplane(type, earthTransform);

        airplane.Initialise(_counter);
        _counter++;

        return airplane;
    }
}
