using Assets.Scripts;
using UnityEngine;

public abstract class Airplane : MonoBehaviour
{
    public string Id { get; private set; }
    
    public AirplaneTypes type { get; private set; }

    public Levels level { get; private set; } 

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
