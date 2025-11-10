using UnityEngine;

public class Location : MonoBehaviour
{
    public string Id;
    public string Name;
    public Vector3 coords;

    public void Initialize(string id, string name, Vector3 coords)
    {
        Id = id;
        Name = name;
        this.coords = coords;
    }
}
