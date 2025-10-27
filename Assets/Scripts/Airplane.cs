using Assets.Scripts;
using UnityEngine;

public abstract class Airplane : MonoBehaviour
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    private static int ctrId;

    public double Range {  get; private set; }
    public int Capacity { get; private set; }
    public double Speed {  get; private set; }



    [SerializeField]
    public GameObject modelPrefab;

    public Airplane()
    {
        Id = ctrId;
        ctrId += 1;
    }

}
