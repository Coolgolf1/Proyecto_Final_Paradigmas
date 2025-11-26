using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HangarCamera : PlayerMovement
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public override void Update()
    {
        _enabled = true;
        base.Update();
    }
}
