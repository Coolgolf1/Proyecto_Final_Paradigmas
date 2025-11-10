using UnityEngine;

public class AirplaneLights : MonoBehaviour
{

    private double elapsedTime = 0;
    private Light airplaneLight;

    void Start()
    {
        airplaneLight = GetComponent<Light>();
    }

    void Update()
    {
        if (elapsedTime > 1)
        {
            airplaneLight.enabled = !airplaneLight.enabled;
            elapsedTime = 0;
        }

        elapsedTime += Time.deltaTime;
    }
}
