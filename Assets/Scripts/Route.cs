using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    public Airport Airport1 { get; private set; }
    public Airport Airport2 { get; private set; }
    public double Distance { get; private set; }
    public List<Vector3> RoutePoints { get; private set; }
    public int IdOfFlightInRoute { get; private set; }

    [SerializeField] private int nSegments = 100;

    public void Initialise(Airport airport1, Airport airport2)
    {
        Airport1 = airport1;
        Airport2 = airport2;
        IdOfFlightInRoute = 1;
    }

    public void SetDistance(double distance)
    {
        if (Distance is 0)
        {
            Distance = distance;
        }
    }

    public void AddIdOfFlight()
    {
        IdOfFlightInRoute++;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = 0;

        List<Vector3> points = GetGreatCirclePoints(Airport1.Location.coords, Airport2.Location.coords, nSegments);
        points = ElevatePoints(points, 0.2f);
        RoutePoints = points;
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
