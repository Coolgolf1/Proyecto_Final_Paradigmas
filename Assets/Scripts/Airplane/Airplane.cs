using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Airplane : MonoBehaviour, IUpgradable, IObject
{
    // Read-only from outside
    public string Id { get; private set; }
    public string TailNumber { get; protected set; }

    public double Range { get; protected set; }
    public int Capacity { get; protected set; }
    public double Speed = GameConstants.relativeSpeed * GameConstants.speedMultiplier;
    public Levels Level { get; protected set; }
    public int Price { get; protected set; }

    [SerializeField] protected TMP_Text tailNumberUI;


    // GameObjects
    private InputAction _clickAction;

    private Camera _camera;

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
        _clickAction = InputSystem.actions.FindAction("Click");
        _camera = _info.playerCamera;
        UIEvents.OnMainMenuEnter.AddListener(_clickAction.Disable);
        UIEvents.OnPlayEnter.AddListener(_clickAction.Enable);
        UIEvents.OnAirplaneStoreEnter.AddListener(_clickAction.Disable);
        UIEvents.OnAirplaneStoreExit.AddListener(_clickAction.Enable);
        UIEvents.OnRouteStoreEnter.AddListener(_clickAction.Disable);
        UIEvents.OnRouteStoreExit.AddListener(_clickAction.Enable);
    }


    private void OnEnable()
    {
        _clickAction.performed += OnClickAirplane;
        _clickAction.Enable();
    }

    private void OnDisable()
    {
        _clickAction.performed -= OnClickAirplane;
    }

    public void Upgrade()
    {
        if (Level < Levels.Elite)
            Level++;

        Speed += (int)(Speed * 0.15);

    }

    private void OnClickAirplane(InputAction.CallbackContext ctx)
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray ray = _camera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Detecta colisión con este objeto
            if (hit.collider.gameObject == this.gameObject)
            {
                _info.airportUI.gameObject.SetActive(false);
                _info.flightUI.gameObject.SetActive(true);
                Flight flight = _info.GetFlightOfAirplane(this);
                _info.flightUI.ShowFlight(flight);
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