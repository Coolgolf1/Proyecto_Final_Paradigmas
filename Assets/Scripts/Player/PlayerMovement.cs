using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public GameObject earth;
    public InputAction drag;
    public InputAction look;
    public InputAction zoom;
    private Vector2 velocity;
    private Vector2 moveValue;
    private Vector2 zoomValue;
    private float modulo;
    [SerializeField] private float zoomFactor;

    [SerializeField]
    private float decay = 8f;

    [SerializeField] private float zoomDecay = 8f;

    [SerializeField] private float zoomSensitivity = 2500f;
    public float sensitivity = 1f;
    [SerializeField] private float moduloPower = 1.8f;

    private float targetZoom = 0;
    private float previousZoom = 0;
    private float actualZoom = 0;

    private void Awake()
    {
        UIEvents.OnMainMenuEnter += DisableActions;
        UIEvents.OnPlayEnter += EnableActions;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        drag = InputSystem.actions.FindAction("IsDragging");
        look = InputSystem.actions.FindAction("Look");
        zoom = InputSystem.actions.FindAction("Zoom");
    }

    public void DisableActions()
    {
        Debug.Log("DISABLED");

        drag.Disable();
        look.Disable();
        zoom.Disable();

    }

    public void EnableActions()
    {
        Debug.Log("ENABLED");
        drag.Enable();
        look.Enable();
        zoom.Enable();

    }

    // Update is called once per frame
    private void Update()
    {
        InertiaDrag();

        zoomValue = zoom.ReadValue<Vector2>();
        modulo = transform.position.magnitude;

        zoomFactor = (Mathf.Pow(modulo, moduloPower) / zoomSensitivity);

        moveValue = velocity;
        transform.RotateAround(earth.transform.position, -Vector3.up, -moveValue[0] * sensitivity * zoomFactor);
        transform.RotateAround(Vector3.zero, transform.right, -moveValue[1] * sensitivity * zoomFactor);

        //Debug.Log(zoomValue.ToString());
        if ((modulo > 30 || zoomValue[1] < 0) && (modulo < 75 || zoomValue[1] > 0))
        {
            targetZoom += zoomValue[1];
        }

        actualZoom = Mathf.Lerp(actualZoom, targetZoom, Time.fixedDeltaTime * zoomDecay);

        transform.position += transform.forward * (actualZoom - previousZoom);

        previousZoom = actualZoom;
    }

    private void InertiaDrag()
    {
        if (drag.IsPressed())
        {
            Debug.Log("DRAG PRESSED");
            moveValue = look.ReadValue<Vector2>();
            velocity = moveValue;
        }
        else
        {
            velocity -= velocity * decay * Time.fixedDeltaTime;
        }
    }
}