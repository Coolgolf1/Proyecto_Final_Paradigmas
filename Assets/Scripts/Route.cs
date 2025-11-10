using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{

    public Airport airport1;
    public Airport airport2;
    public double distance;

    public List<Airplane> airplanes;

    [SerializeField] private int nSegments = 20;
    public List<Vector3> routePoints;

    public void AddPlaneToRoute(Airplane airplane)
    {
        airplanes.Add(airplane);
    }

    public void RemovePlaneFromRoute(Airplane airplane)
    {
        airplanes.Remove(airplane);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = 0;

        var points = GetGreatCirclePoints(airport1.Location.coords, airport2.Location.coords, nSegments);
        points = ElevatePoints(points, 0.2f);
        routePoints = points;
        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());

    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Vector3> GetGreatCirclePoints(Vector3 A, Vector3 B, int segments)
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

    public List<Vector3> ElevatePoints(List<Vector3> points, float maxElevation)
    {
        List<Vector3> elevated = new List<Vector3>();
        int n = points.Count;
        for (int i = 0; i < n; i++)
        {
            float t = (float)i / (n - 1);
            float elevation = Mathf.Sin(t * Mathf.PI) * maxElevation;
            Vector3 elevatedPoint = points[i].normalized * (points[i].magnitude * (1 + elevation));
            elevated.Add(elevatedPoint);
        }
        return elevated;
    }


}
