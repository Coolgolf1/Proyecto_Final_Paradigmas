using UnityEngine;

public class AirplaneLights : MonoBehaviour
{
    private double _elapsedTime = 0;
    private Light _airplaneLight;

    private void Start()
    {
        _airplaneLight = GetComponent<Light>();
    }

    private void Update()
    {
        if (_elapsedTime > 1)
        {
            _airplaneLight.enabled = !_airplaneLight.enabled;
            _elapsedTime = 0;
        }

        _elapsedTime += Time.deltaTime;
    }
}