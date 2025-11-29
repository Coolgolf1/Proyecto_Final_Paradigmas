using UnityEngine;

public class AirplaneSpawner : MonoBehaviour
{
    [SerializeField] private GameObject SmallAirplane;
    [SerializeField] private GameObject MediumAirplane;
    [SerializeField] private GameObject LargeAirplane;

    public GameObject GetPrefab(AirplaneTypes type)
    {
        switch (type)
        {
            case AirplaneTypes.Small:
                return SmallAirplane;

            case AirplaneTypes.Medium:
                return MediumAirplane;

            case AirplaneTypes.Large:
                return LargeAirplane;
        }

        return null;
    }

    public Airplane InstantiateAirplane<T>(GameObject prefab, Transform earthTransform) where T : Airplane
    {
        Airplane airplane = Instantiate(prefab, earthTransform).GetComponent<T>();

        return airplane;
    }
}