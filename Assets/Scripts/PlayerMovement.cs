using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public GameObject earth;
    public float sensitivity;
    public InputAction look;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        look = InputSystem.actions.FindAction("Look");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveValue = look.ReadValue<Vector2>();
        
        transform.RotateAround(earth.transform.position, -Vector3.up, -moveValue[0] * sensitivity); //use transform.Rotate(-transform.up * rotateHorizontal * sensitivity) instead if you dont want the camera to rotate around the player
        transform.RotateAround(Vector3.zero, transform.right, -moveValue[1] * sensitivity); // again, use transform.Rotate(transform.right * rotateVertical * sensitivity) if you don't want the camera to rotate around the player

    }
}
