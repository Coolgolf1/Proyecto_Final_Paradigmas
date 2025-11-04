using System.Collections.Generic;

public static class RouteAssigner
{
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

    private static (Airplane, double) GetFastestAirplaneAndTime(Airport origin, Edge edge)
    {
        // Compute the shortest possible time (in hours) among all airplanes
        double bestTime = double.PositiveInfinity;
        Airplane bestAirplane = null;

        foreach (Airplane airplane in Info.airplanes)
        {
            if (airplane.Speed <= 0) continue;

            // Get Takeoff Airplane
            Airport takeoffAirport = Info.GetTakeoffAirportOfAirplane(airplane);

            if (takeoffAirport == null)
                continue;

            double distance = 0;

            // Flying away from origin airport
            if (takeoffAirport == origin)
            {
                // Add distance of coming back to origin airport
                distance += edge.distance;

                // Add remaining distance to destination airport
                distance += (1 - Info.GetFlightOfAirplane(airplane).FlightProgress) * edge.distance;
            }
            else
            {
                // Add remaining distance to destination airport
                distance += (1 - Info.GetFlightOfAirplane(airplane).FlightProgress) * edge.distance;
            }

            double flightTime = distance / airplane.Speed;

            if (flightTime < bestTime)
            {
                bestTime = flightTime;
                bestAirplane = airplane;
            }
        }

        return (bestAirplane, bestTime);
    }

    public static Airplane Dijkstra(
          Dictionary<Airport, List<Edge>> graph,
          Airport start,
          Airport end)
    {
        Dictionary<Airport, double> timeFromStart = new Dictionary<Airport, double>();
        Dictionary<Airport, Airport> previous = new Dictionary<Airport, Airport>();
        List<Airport> queue = new List<Airport>(graph.Keys);
        Dictionary<Airport, Airplane> airplaneUsed = new Dictionary<Airport, Airplane>();

        foreach (Airport node in graph.Keys)
            timeFromStart[node] = double.PositiveInfinity;

        timeFromStart[start] = 0;

        while (queue.Count > 0)
        {
            queue.Sort((a, b) => timeFromStart[a].CompareTo(timeFromStart[b]));
            Airport current = queue[0];
            queue.RemoveAt(0);

            if (current == end)
                break;

            if (!graph.ContainsKey(current))
                continue;

            foreach (Edge edge in graph[current])
            {
                (Airplane airplane, double cost) = GetFastestAirplaneAndTime(current, edge);
                if (airplane is null) continue;
                if (double.IsInfinity(cost)) continue;

                double alt = timeFromStart[current] + cost;
                if (alt < timeFromStart[edge.To])
                {
                    timeFromStart[edge.To] = alt;
                    previous[edge.To] = current;
                    airplaneUsed[edge.To] = airplane;
                }
            }
        }

        // If nothing found
        if (!previous.ContainsKey(end) && start != end)
            return null;

        // Reconstruct path
        List<Airport> path = new List<Airport>();
        Airport u = end;
        while (previous.ContainsKey(u))
        {
            path.Insert(0, u);
            u = previous[u];
        }
        path.Insert(0, start);

        Airport nextHop = path.Count >= 2 ? path[1] : null;

        Airplane nextAirplane = null;
        if (nextHop != null)
            airplaneUsed.TryGetValue(nextHop, out nextAirplane);

        return nextAirplane;
    }
}