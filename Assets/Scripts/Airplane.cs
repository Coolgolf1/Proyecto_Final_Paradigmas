using UnityEngine;
using System.Collections.Generic;
using System;

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

    public (double, double) UpdatePosition(List<Vector3> routePoints, double distance, double elapsedKM)
    {
        float indexProgress;
        int targetIndex; 

        elapsedKM += Speed * Time.deltaTime;
        double flightProgress = elapsedKM / distance;

        if (flightProgress < 1)
        {
            indexProgress = (float)(flightProgress * (routePoints.Count - 1));

            targetIndex = (int)Mathf.Floor(indexProgress);

            transform.position = Vector3.Lerp(routePoints[targetIndex], routePoints[targetIndex + 1], indexProgress - targetIndex);
            transform.LookAt(routePoints[targetIndex + 1], transform.position - Vector3.zero);
        }

        return (elapsedKM, flightProgress);
    }
}
