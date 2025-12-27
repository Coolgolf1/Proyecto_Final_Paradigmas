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

            double tempDistance;

            Flight flight = _info.GetFlightOfAirplane(airplane);

            // 1. Flight not yet created
            if (flight is null)
                tempDistance = distance;
            else
            {
                // 2. Do not check full planes
                if (airplane.Capacity <= flight.GetNumberOfPassengers())
                    continue;

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
                if (distance <= airplane.Range)
                {
                    bestAirplane = airplane;
                    bestDistance = tempDistance;
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

        Airport partialFarthest = start;
        double bestReached = 0;

        Airport closestAirport = start;
        double minDirectDistanceToEnd = Auxiliary.GetDirectDistanceBetweenAirports(start, end);

        while (queue.Count > 0)
        {
            Airport current = queue.Dequeue();
            double currentDistance = distanceFromStart[current];

            double distToDest = Auxiliary.GetDirectDistanceBetweenAirports(current, end);
            if (distToDest < minDirectDistanceToEnd)
            {
                minDirectDistanceToEnd = distToDest;
                closestAirport = current;
            }

            if (current == end) break;
            if (processed.Contains(current)) continue;
            processed.Add(current);

            if (!DijkstraGraph.graph.ContainsKey(current))
                continue;

            if (previous.ContainsKey(current) || current == start)
            {
                partialFarthest = current;
                bestReached = currentDistance;
            }

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

        Airport destinationFound = end;

        // If nothing found
        if (!previous.ContainsKey(end))
        {
            if (closestAirport == start)
                return (null, null);

            destinationFound = closestAirport;
        }

        // Reconstruct path
        List<Airport> path = new List<Airport>();
        Airport u = destinationFound;
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