using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

public class Route : MonoBehaviour
{

    public Airport airport1;
    public Airport airport2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LineRenderer line = gameObject.AddComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;
        line.material = new Material(Shader.Find("Materials/ColorRojo"));
        line.positionCount = 0;

        var points = GetGreatCirclePoints(airport1.location.coords, airport2.location.coords, 3);
        //points = ElevatePoints(points, elevation);

        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    List<Vector3> GetGreatCirclePoints(Vector3 A, Vector3 B, int segments)
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            Vector3 point = Vector3.Slerp(A.normalized, B.normalized, t) * A.magnitude;
            points.Add(point);
        }
        return points;
    }
}
