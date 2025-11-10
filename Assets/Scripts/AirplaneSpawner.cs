using UnityEngine;

public class AirplaneSpawner : MonoBehaviour
{
    [SerializeField] GameObject SmallAirplanePrefab;
    [SerializeField] GameObject MediumAirplanePrefab;
    [SerializeField] GameObject LargeAirplanePrefab;

    public void Awake()
    {

    }

    public Airplane InstantiateAirplane(AirplaneTypes type, Transform earthTransform)
    {
        Airplane airplane;

        switch (type)
        {
            case AirplaneTypes.Small:
                airplane = Instantiate(SmallAirplanePrefab, earthTransform).GetComponent<AirplaneSmall>();
                break;

            case AirplaneTypes.Medium:
                airplane = Instantiate(MediumAirplanePrefab, earthTransform).GetComponent<AirplaneMedium>();
                break;

            case AirplaneTypes.Large:
                airplane = Instantiate(LargeAirplanePrefab, earthTransform).GetComponent<AirplaneLarge>();
                break;

            default:
                Debug.Log("ERROR CREATING AIRPLANE (WRONG TYPE), USING DEFAULT.");
                airplane = null;
                break;
        }

        return airplane;
    }
}
