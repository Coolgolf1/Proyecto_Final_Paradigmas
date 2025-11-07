using UnityEngine;

public class AirplaneLights : MonoBehaviour
{

    private double elapsedTime = 0;

    private Light airplaneLight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        airplaneLight = GetComponent<Light>();
    }

    // Update is called once per frame
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
