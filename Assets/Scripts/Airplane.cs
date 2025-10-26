using Assets.Scripts;
using UnityEngine;

public abstract class Airplane : MonoBehaviour
{
    public int Id { get; private set; }
    private static int ctrId;

    public Levels Level { get; private set; }

    [SerializeField]
    public GameObject modelPrefab;

    public Airplane(Levels level)
    {
        Id = ctrId;
        ctrId += 1;
    }
}
