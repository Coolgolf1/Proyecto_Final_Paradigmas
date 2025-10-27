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
    private double modulo;
    private float zoomFactor;
    private float distance;
    [SerializeField]
    private float decay = 8f;
    private float zoomSensitivity = 100f;
    public float sensitivity = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        drag = InputSystem.actions.FindAction("IsDragging");
        look = InputSystem.actions.FindAction("Look");
        zoom = InputSystem.actions.FindAction("Zoom");
    }

    // Update is called once per frame
    void Update()
    {
        InertiaDrag();

        zoomValue = zoom.ReadValue<Vector2>();
        modulo = transform.position.magnitude;

        zoomFactor = (float)(modulo / zoomSensitivity);

        moveValue = velocity;
        transform.RotateAround(earth.transform.position, -Vector3.up, -moveValue[0] * sensitivity * zoomFactor);
        transform.RotateAround(Vector3.zero, transform.right, -moveValue[1] * sensitivity * zoomFactor);


        //Debug.Log(zoomValue.ToString());
        if ((modulo > 30 || zoomValue[1] < 0) && (modulo < 75 || zoomValue[1] > 0))
        {
            transform.position += zoomValue[1] * transform.forward;
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
            velocity -= velocity * decay * Time.deltaTime;
        }
    }
}
