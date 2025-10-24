using Assets.Scripts;
using UnityEngine;

public abstract class Airport : MonoBehaviour
{
    public string Id { get; private set; }

    public string Name { get; private set; }

    public string City { get; private set; }

    public Levels level { get; private set; }

    public Vector3 Location;
    public GameObject modelPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
