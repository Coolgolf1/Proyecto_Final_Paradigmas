using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Airplane : MonoBehaviour, IUpgradable
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public double Range { get; private set; }
    public int Capacity { get; private set; }
    public double Speed { get; protected set; }
    public Levels Level { get; private set; }

    private InputAction clickAction;
    private Camera cam;

    private InfoSingleton info = InfoSingleton.GetInstance();

    public void Initialise(int id)
    {
        Id = id;
        Level = Levels.Basic;
    }

    public virtual void Awake()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        cam = info.playerCamera;

        // Define base properties
        Range = 10000;
        Capacity = 100;
        Speed = 300;
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

    public void Upgrade()
    {
        if (Level < Levels.Elite)
            Level++;
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
