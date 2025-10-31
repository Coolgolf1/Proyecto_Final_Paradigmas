using UnityEngine;

public class Location : MonoBehaviour
{
    public string id;
    public Vector3 coords;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = coords;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
