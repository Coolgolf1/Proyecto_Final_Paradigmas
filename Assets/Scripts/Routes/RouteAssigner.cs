using System.Collections.Generic;

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
        // Compute the shortest possible time (in hours) among all airplanes
        double bestDistance = double.PositiveInfinity;
        Airplane bestAirplane = null;

        foreach (Airplane airplane in start.Hangar)
        {
            double distance = Auxiliary.GetDirectDistanceBetweenAirports(origin, edge.To);

            if (distance > airplane.Range)
                continue;

            double tempDistance;

            Flight flight = _info.GetFlightOfAirplane(airplane);

            // Do not check full planes
            if (flight is not null)
            {
                if (airplane.Capacity <= flight.GetNumberOfPassengers())
                {
                    continue;
                }
            }

            // 1. Flight assigned but in Hangar
            if (flight is not null)
            {
                Airport airportDest = flight.AirportDest;

                if (airportDest == edge.To)
                {
                    tempDistance = 0;
                }
                else
                {
                    tempDistance = flight.Route.Distance + Auxiliary.GetDirectDistanceBetweenAirports(airportDest, end);
                }
            }
            // 2. Flight not yet created
            else
            {
                tempDistance = Auxiliary.GetDirectDistanceBetweenAirports(origin, end);
            }

            if (tempDistance > Auxiliary.GetDirectDistanceBetweenAirports(origin, end))
                continue;

            // Choose best airplane with distance
            if (tempDistance <= bestDistance)
            {
                bestDistance = tempDistance;
                bestAirplane = airplane;
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

        
        // Initialize
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

            if (current == end)
                break;

            if (!DijkstraGraph.graph.ContainsKey(current))
                continue;

            if (processed.Contains(current))
                continue;

            processed.Add(current);

            foreach (Edge edge in DijkstraGraph.graph[current])
            {
                (Airplane airplane, double cost) = GetFastestAirplaneAndTime(current, edge, start, end);

                if (airplane is null) continue;
                if (double.IsInfinity(cost)) continue;

                Airport neighbor = edge.To;
                double newDistance = currentDistance + cost;

                if (newDistance <= distanceFromStart[neighbor])
                {
                    distanceFromStart[neighbor] = newDistance;
                    previous[neighbor] = current;
                    airplaneUsed[neighbor] = airplane;
                    queue.Enqueue(neighbor, (int)newDistance);
                }
            }
        }

        // If nothing found
        if (!previous.ContainsKey(end) && start != end)
        {
            return (null, null);
        }

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