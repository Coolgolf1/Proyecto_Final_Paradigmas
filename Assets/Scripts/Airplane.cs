using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Airplane : MonoBehaviour
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    private static int ctrId;

    public double Range { get; private set; }
    public int Capacity { get; private set; }
    public double Speed { get; protected set; }

    //[SerializeField]
    //public GameObject modelPrefab;

    //public Airplane()

    private InputAction clickAction;
    private Camera cam;

    private InfoSingleton info = InfoSingleton.GetInstance();

    public virtual void Awake()
    {
        Id = ctrId;
        ctrId += 1;
        Capacity = 100;
        clickAction = InputSystem.actions.FindAction("Click");
        cam = info.playerCamera;
    }

    private void OnEnable()
    {
        clickAction.performed += OnClickAirplane;
        //clickAction.Enable();
    }

    private void OnDisable()
    {
        clickAction.performed -= OnClickAirplane;
        //clickAction.Disable();
    }

    private void OnClickAirplane(InputAction.CallbackContext ctx)
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            
            // Detecta colisión con este objeto
            if (hit.collider.gameObject == this.gameObject)
            {
                info.airportUI.gameObject.SetActive(false);
                info.flightUI.gameObject.SetActive(true);
                info.flightUI.ShowFlight(info.GetFlightOfAirplane(this));
                
            }

        }
    }

    public (double, double) UpdatePosition(List<Vector3> routePoints, double distance, double elapsedKM)
    {
        float indexProgress;
        int targetIndex; 

        elapsedKM += Speed * Time.deltaTime;
        double flightProgress = elapsedKM / distance;

        if (flightProgress < 1)
        {
            indexProgress = (float)(flightProgress * (routePoints.Count - 1));

            targetIndex = (int)Mathf.Floor(indexProgress);

            transform.position = Vector3.Lerp(routePoints[targetIndex], routePoints[targetIndex + 1], indexProgress - targetIndex);
            transform.LookAt(routePoints[targetIndex + 1], transform.position - Vector3.zero);
        }

        return (elapsedKM, flightProgress);
    }
}
