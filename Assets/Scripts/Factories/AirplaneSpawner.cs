using UnityEngine;

public class AirplaneSpawner : MonoBehaviour
{
    [SerializeField] private GameObject SmallAirplanePrefab;
    [SerializeField] private GameObject MediumAirplanePrefab;
    [SerializeField] private GameObject LargeAirplanePrefab;

    public GameObject GetPrefab(AirplaneTypes type)
    {
        switch (type)
        {
            case AirplaneTypes.Small:
                return SmallAirplanePrefab;

            case AirplaneTypes.Medium:
                return MediumAirplanePrefab;

            case AirplaneTypes.Large:
                return LargeAirplanePrefab;
        }

        return null;
    }

    public Airplane InstantiateAirplane<T>(GameObject prefab, Transform earthTransform) where T : Airplane
    {
        Airplane airplane = Instantiate(prefab, earthTransform).GetComponent<T>();

        return airplane;
    }
}