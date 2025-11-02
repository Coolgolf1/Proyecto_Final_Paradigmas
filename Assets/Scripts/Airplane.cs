using Assets.Scripts;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public abstract class Airplane : MonoBehaviour
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    private static int ctrId;

    public double Range {  get; private set; }
    public int Capacity { get; private set; }
    public double Speed {  get; protected set; }

    public double FlightProgress { get; private set; }
    public double ElapsedKM { get; private set; }
    private int targetIndex;
    private float indexProgress;

    //[SerializeField]
    //public GameObject modelPrefab;

    //public Airplane()
    public virtual void Awake()
    {
        Id = ctrId;
        ctrId += 1;
        FlightProgress = 0;
        ElapsedKM = 0;
    }

    public bool UpdatePosition(List<Vector3> routePositions, double totalDistance)
    {
        if (FlightProgress < 1)
        {
            ElapsedKM += Speed * Time.deltaTime;
            FlightProgress = ElapsedKM / totalDistance;

            indexProgress = (float)(FlightProgress * (routePositions.Count - 1));

            targetIndex = (int)Mathf.Floor(indexProgress);
            
            this.transform.position = Vector3.Lerp(routePositions[targetIndex], routePositions[targetIndex + 1], indexProgress - targetIndex);
            this.transform.LookAt(routePositions[targetIndex + 1], this.transform.position - Vector3.zero);

            return true;
        }
        else
        {
            return false;
        }
    }
}
