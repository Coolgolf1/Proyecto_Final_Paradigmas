using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject reference;
    protected InputAction drag;
    protected InputAction look;
    protected InputAction zoom;
    private Vector2 velocity;
    private Vector2 moveValue;


    [SerializeField] private float decay = 8f;

    [SerializeField] private float sensitivity = 1f;
    [SerializeField] protected float zoomFactor;


    protected bool _enabled = false;
    protected InfoSingleton _info = InfoSingleton.GetInstance();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        drag = InputSystem.actions.FindAction("IsDragging");
        look = InputSystem.actions.FindAction("Look");
        zoom = InputSystem.actions.FindAction("Zoom");
        UIEvents.OnMainMenuEnter.AddListener(DisableActions);
        UIEvents.OnPlayEnter.AddListener(EnableActions);
        UIEvents.OnAirplaneStoreEnter.AddListener(DisableActions);
        UIEvents.OnAirplaneStoreExit.AddListener(EnableActions);
        UIEvents.OnRouteStoreEnter.AddListener(DisableActions);
        UIEvents.OnRouteStoreExit.AddListener(EnableActions);
        UIEvents.OnEndGameEnter.AddListener(DisableActions);
        UIEvents.OnEndGameExit.AddListener(DisableActions);
    }

    public void DisableActions()
    {
        drag.Disable();
        look.Disable();
        zoom.Disable();
        _enabled = false;

    }

    public void EnableActions()
    {
        drag.Enable();
        look.Enable();
        zoom.Enable();
        _enabled = true;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (_enabled)
        {
            InertiaDrag();

            moveValue = velocity;
            transform.RotateAround(reference.transform.position, -Vector3.up, -moveValue[0] * sensitivity * zoomFactor);
            transform.RotateAround(Vector3.zero, transform.right, -moveValue[1] * sensitivity * zoomFactor);

        }

    }

    private void InertiaDrag()
    {
        if (drag.IsPressed())
        {
            moveValue = look.ReadValue<Vector2>();
            velocity = moveValue;
        }
        else
        {
            velocity -= velocity * decay * Time.fixedDeltaTime;
        }
    }
}