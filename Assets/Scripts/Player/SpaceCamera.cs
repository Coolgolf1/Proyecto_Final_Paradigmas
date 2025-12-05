using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SpaceCamera : PlayerMovement
{
    private Vector2 zoomValue;
    private float modulo;
    [SerializeField] private float moduloPower = 1.8f;
    [SerializeField] private float zoomDecay = 8f;
    [SerializeField] private float zoomSensitivity = 2500f;
    
    private float targetZoom = 0;
    private float previousZoom = 0;
    private float actualZoom = 0;

    private Airplane followingAirplane;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public void Start()
    //{
        
        
    //}
    
    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (_enabled)
        {
            SmoothZoom();
        }

        if (followingAirplane is not null)
        {
            FollowAirplane();
        }
    }

    private void SmoothZoom()
    {
        zoomValue = zoom.ReadValue<Vector2>();
        modulo = transform.position.magnitude;

        zoomFactor = (Mathf.Pow(modulo, moduloPower) / zoomSensitivity);

        if ((modulo > 30 || zoomValue[1] < 0) && (modulo < 75 || zoomValue[1] > 0))
        {
            targetZoom += zoomValue[1];
        }

        actualZoom = Mathf.Lerp(actualZoom, targetZoom, Time.fixedDeltaTime * zoomDecay);

        transform.position += transform.forward * (actualZoom - previousZoom);

        previousZoom = actualZoom;
    }

    public void SetAirplane(Airplane airplane)
    {
        followingAirplane = airplane;
    }

    private void FollowAirplane()
    {
        if (drag.IsPressed())
        {
            followingAirplane = null;
            return;
        }


        Vector3 centroTierra = _info.earth.transform.position;

        // Posición del avión
        Vector3 posicionAvion = followingAirplane.transform.position;

        // Vector desde la tierra hacia el avión
        Vector3 direccion = (posicionAvion - centroTierra).normalized;

        // Distancia deseada de la cámara respecto al avión (puedes ajustar este valor)
        float distanciaSobreAvion = 20f;

        // Nueva posición de la cámara: sobre el avión, en la misma dirección que el centro de la tierra
        Vector3 posicionCamara = posicionAvion + direccion * distanciaSobreAvion;

        // Sitúa la cámara en la posición calculada
        transform.position = posicionCamara;

        transform.LookAt(_info.earth.transform.position);
    }
}
