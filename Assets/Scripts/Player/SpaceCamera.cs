using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
}
