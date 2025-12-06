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
    private bool _arrived;
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
        else
        {
            _arrived = false;
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


        Vector3 earthCenter = _info.earth.transform.position;

        Vector3 airplanePos = followingAirplane.transform.position;

        Vector3 direction = (airplanePos - earthCenter).normalized;
        float distanceOver = 15f;

        Vector3 objPosition = airplanePos + direction * distanceOver;

        if (!_arrived)
        {
            transform.position = Vector3.Slerp(transform.position, objPosition, Time.fixedDeltaTime);
            //Quaternion finalRotation = Quaternion.LookRotation(direction, Vector3.up);
            //transform.rotation = Quaternion.Lerp(transform.rotation, finalRotation, 3 * Time.deltaTime);

            // If not close enough to initial view
            if ((objPosition - transform.position).magnitude < 0.5)
                _arrived = true;

        }

        else
        {
            transform.position = objPosition;
            
        }
        transform.LookAt(_info.earth.transform.position);
    }
}
