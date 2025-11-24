using UnityEngine;

public class AirplaneFactory : ITypedFactory<IObject, AirplaneTypes>
{
    private int _counter = 0;

    private static AirplaneFactory _instance;

    private AirplaneSpawner _spawner;

    private AirplaneFactory()
    { }

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
        GameEvents.OnTransitionExit.AddListener(ResetCounter);
    }

    public IObject Build(AirplaneTypes type, Transform earthTransform)
    {
        GameObject prefab = _spawner.GetPrefab(type);
        Airplane airplane;

        switch (type)
        {
            case AirplaneTypes.Small:
                airplane = _spawner.InstantiateAirplane<AirplaneSmall>(prefab, earthTransform);
                break;

            case AirplaneTypes.Medium:
                airplane = _spawner.InstantiateAirplane<AirplaneMedium>(prefab, earthTransform);
                break;

            case AirplaneTypes.Large:
                airplane = _spawner.InstantiateAirplane<AirplaneLarge>(prefab, earthTransform);
                break;

            default:
                Debug.Log("type not set");
                return null;
        }

        airplane.Initialise(_counter);
        _counter++;

        return airplane;
    }

    public void ResetCounter()
    {
        _counter = 0;
    }
}