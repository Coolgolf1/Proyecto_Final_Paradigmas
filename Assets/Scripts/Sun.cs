using UnityEngine;

public class Sun : MonoBehaviour
{
    [SerializeField] private float speed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(-Vector3.right * Time.deltaTime * speed, Space.Self);
    }
}