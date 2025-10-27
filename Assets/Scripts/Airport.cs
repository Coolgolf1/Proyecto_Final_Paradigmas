using Assets.Scripts;
using UnityEngine;

public class Airport : MonoBehaviour
{
    public string Id { get; private set; }
    public string Name { get; private set; }

    [SerializeField]
    public Location location;
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
