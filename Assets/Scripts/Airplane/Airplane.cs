using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public abstract class Airplane : MonoBehaviour, IUpgradable, IObject
{
    // Read-only from outside
    public string Id { get; private set; }
    public string Name { get; private set; }
    public double Range { get; protected set; }
    public int Capacity { get; protected set; }
    public double Speed { get; protected set; } = GameConstants.relativeSpeed * GameConstants.speedMultiplier;
    public Levels Level { get; protected set; }
    public int Price { get; protected set; }

    [SerializeField] protected TMP_Text tailNumber;


    // GameObjects
    private InputAction clickAction;

    private Camera cam;

    // Dependencies
    private InfoSingleton _info = InfoSingleton.GetInstance();

    public abstract void SetTailNumber();

    public void Initialise(int id)
    {
        Id = $"{id:D4}";
        
        Level = Levels.Basic;
        SetTailNumber();
    }

    public virtual void Awake()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        cam = _info.playerCamera;
        UIEvents.OnMainMenuEnter.AddListener(clickAction.Disable);
        UIEvents.OnPlayEnter.AddListener(clickAction.Enable);
    }


    private void OnEnable()
    {
        clickAction.performed += OnClickAirplane;
        clickAction.Enable();
    }

    private void OnDisable()
    {
        clickAction.performed -= OnClickAirplane;
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
                _info.airportUI.gameObject.SetActive(false);
                _info.flightUI.gameObject.SetActive(true);
                _info.flightUI.ShowFlight(_info.GetFlightOfAirplane(this));
                _info.GetRouteOfAirplane(this).LitRoute();
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