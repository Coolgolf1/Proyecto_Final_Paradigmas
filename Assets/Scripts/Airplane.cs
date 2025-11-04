using UnityEngine;

public abstract class Airplane : MonoBehaviour
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    private static int ctrId;

    public double Range { get; private set; }
    public int Capacity { get; private set; }
    public double Speed { get; protected set; }

    //[SerializeField]
    //public GameObject modelPrefab;

    //public Airplane()
    public virtual void Awake()
    {
        Id = ctrId;
        ctrId += 1;
        Capacity = 100;
    }
}
