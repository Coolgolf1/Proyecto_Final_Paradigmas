using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public abstract class AirplaneFactory : ITypedFactory<IObject, AirplaneTypes>
{
    protected AirplaneSpawner _spawner;
    protected int _counter = 0;
    public void Initialise(AirplaneSpawner spawner)
    {
        _spawner = spawner;
        GameEvents.OnTransitionExit.AddListener(ResetCounter);
    }

    public abstract IObject Build(Transform earthTransform);

    protected void ResetCounter() => _counter = 0;

}

public class SmallAirplaneFactory : AirplaneFactory
{
    private static SmallAirplaneFactory _instance;

    private SmallAirplaneFactory() { }

    public static SmallAirplaneFactory GetInstance()
    {
        if (_instance == null)
        {
            _instance = new SmallAirplaneFactory();
        }
        return _instance;
    }

    public override IObject Build(Transform earthTransform)
    {
        GameObject prefab = _spawner.GetPrefab(AirplaneTypes.Small);
        Airplane airplane;
        airplane = _spawner.InstantiateAirplane<AirplaneSmall>(prefab, earthTransform);

        airplane.Initialise(_counter);
        _counter++;

        return airplane;
    }
    
}


public class MediumAirplaneFactory : AirplaneFactory
{
    private static MediumAirplaneFactory _instance;
    
    private MediumAirplaneFactory() { }

    public static MediumAirplaneFactory GetInstance()
    {
        if (_instance == null)
        {
            _instance = new MediumAirplaneFactory();
        }
        return _instance;
    }

    public override IObject Build(Transform earthTransform)
    {
        GameObject prefab = _spawner.GetPrefab(AirplaneTypes.Medium);
        Airplane airplane;
        airplane = _spawner.InstantiateAirplane<AirplaneMedium>(prefab, earthTransform);

        airplane.Initialise(_counter);
        _counter++;

        return airplane;
    }

}

public class LargeAirplaneFactory : AirplaneFactory
{
    private static LargeAirplaneFactory _instance;

    private LargeAirplaneFactory() { }

    public static LargeAirplaneFactory GetInstance()
    {
        if (_instance == null)
        {
            _instance = new LargeAirplaneFactory();
        }
        return _instance;
    }

    public override IObject Build(Transform earthTransform)
    {
        GameObject prefab = _spawner.GetPrefab(AirplaneTypes.Large);
        Airplane airplane;
        airplane = _spawner.InstantiateAirplane<AirplaneLarge>(prefab, earthTransform);

        airplane.Initialise(_counter);
        _counter++;

        return airplane;
    }
}