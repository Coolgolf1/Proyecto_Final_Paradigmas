using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    public Airport To { get; private set; }
    public double distance { get; private set; }

    public Edge(Airport to, double distance)
    {
        To = to;
        this.distance = distance;
    }
}

public static class RouteAssigner
{
    private static InfoSingleton _info = InfoSingleton.GetInstance();

    private static (Airplane, double) GetFastestAirplaneAndTime(Airport origin, Edge edge, Airport start, Airport end)
    {

        double distance = Auxiliary.GetDirectDistanceBetweenAirports(origin, edge.To);
        // Compute the shortest possible time (in hours) among all airplanes
        double bestDistance = distance;
        Airplane bestAirplane = null;

        foreach (Airplane airplane in start.Hangar)
        {
            
            double tempDistance;

            Flight flight = _info.GetFlightOfAirplane(airplane);

            // 1. Flight not yet created
            if (flight is null)
                tempDistance = distance;
            else
            {
                // 2. Do not check full planes
                if (airplane.Capacity <= flight.GetNumberOfPassengers())
                {
                    continue;
                }

                // 3. Flight assigned but in Hangar
                Airport airportDest = flight.AirportDest;
                if (airportDest == edge.To)
                {
                    tempDistance = flight.Route.Distance;
                }
                else
                {
                    continue;
                }
            }

            // Choose best airplane with distance
            if (tempDistance <= bestDistance)
            {
                bestDistance = tempDistance;
                if (distance <= airplane.Range)
                {
                    bestAirplane = airplane;
                    
                }
            }
        }

        return (bestAirplane, bestDistance);
    }

    public static Airport GetNextHop(List<Airport> path)
    {
        return path.Count >= 2 ? path[1] : null;
    }

    public static (Airplane, List<Airport>) Dijkstra(
          Airport start,
          Airport end)
    {
        Dictionary<Airport, double> distanceFromStart = new Dictionary<Airport, double>();
        Dictionary<Airport, Airport> previous = new Dictionary<Airport, Airport>();
        PriorityQueue<Airport> queue = new PriorityQueue<Airport>();
        Dictionary<Airport, Airplane> airplaneUsed = new Dictionary<Airport, Airplane>();

        
        // Initialize weights
        foreach (Airport node in DijkstraGraph.graph.Keys)
        {
            distanceFromStart[node] = double.PositiveInfinity;
        }

        distanceFromStart[start] = 0;
        queue.Enqueue(start, 0);

        HashSet<Airport> processed = new HashSet<Airport>();

       
        while (queue.Count > 0)
        {
            Airport current = queue.Dequeue();
            double currentDistance = distanceFromStart[current];

            if (current == end) break;
            if (processed.Contains(current)) continue;
            processed.Add(current);

            if (!DijkstraGraph.graph.ContainsKey(current))
                continue;


            foreach (Edge edge in DijkstraGraph.graph[current])
            {
                (Airplane airplane, double cost) = GetFastestAirplaneAndTime(current, edge, start, end);

                if (double.IsInfinity(cost)) continue;

                Airport neighbor = edge.To;
                double newDistance = currentDistance + cost;

                if (newDistance <= distanceFromStart[neighbor])
                {
                    distanceFromStart[neighbor] = newDistance;
                    previous[neighbor] = current;
                    if (airplane is not null)
                    {
                        airplaneUsed[neighbor] = airplane;
                    }
                    queue.Enqueue(neighbor, (int)newDistance);
                }
            }
        }

        

        // If nothing found
        if (!previous.ContainsKey(end)) 
           return (null, null);
        // Reconstruct path
        List<Airport> path = new List<Airport>();
        Airport u = end;
        while (previous.ContainsKey(u))
        {
            path.Insert(0, u);
            u = previous[u];
        }
        path.Insert(0, start);
        
        
        Airport nextHop = GetNextHop(path);

        Airplane nextAirplane = null;
        if (nextHop != null)
            airplaneUsed.TryGetValue(nextHop, out nextAirplane);
        
        return (nextAirplane, path);
    }
}