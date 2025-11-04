using System;
using System.Collections.Generic;

public class RouteAssigner
{
    public class Edge
    {
        public Airport To { get; }
        public double DistanceKm { get; }

        public Edge(Airport to, double distanceKm)
        {
            To = to;
            DistanceKm = distanceKm;
        }
    }

    public (Airport? nextHop, double totalHours) DijkstraWithPlanes(
          Dictionary<Airport, List<Edge>> graph,
          Airport start,
          Airport end,
          List<Airplane> airplanes,
          Func<Airport, Airport, double> distanceKm)
    {
        var timeFromStart = new Dictionary<Airport, double>();
        var previous = new Dictionary<Airport, Airport>();
        var queue = new List<Airport>(graph.Keys);

        foreach (var node in graph.Keys)
            timeFromStart[node] = double.PositiveInfinity;

        timeFromStart[start] = 0;

        double EdgeTimeHours(Airport origin, Edge edge)
        {
            // Compute the shortest possible time (in hours) among all airplanes
            double best = double.PositiveInfinity;

            foreach (var plane in airplanes)
            {
                if (plane.Speed <= 0) continue;

                // Distance from plane's current position to the origin airport
                Airport currentLocation = plane.CurrentAirport;

                if (currentLocation == null)
                    continue;

                double ferryDistance = distanceKm(currentLocation, origin);
                double timeToOrigin = ferryDistance / plane.Speed;

                // Flight time itself
                double flightTime = edge.DistanceKm / plane.Speed;

                double total = timeToOrigin + flightTime;
                if (total < best)
                    best = total;
            }

            return best;
        }

        while (queue.Count > 0)
        {
            queue.Sort((a, b) => timeFromStart[a].CompareTo(timeFromStart[b]));
            var current = queue[0];
            queue.RemoveAt(0);

            if (current == end)
                break;

            if (!graph.ContainsKey(current))
                continue;

            foreach (var edge in graph[current])
            {
                double cost = EdgeTimeHours(current, edge);
                if (double.IsInfinity(cost)) continue;

                double alt = timeFromStart[current] + cost;
                if (alt < timeFromStart[edge.To])
                {
                    timeFromStart[edge.To] = alt;
                    previous[edge.To] = current;
                }
            }
        }

        if (!previous.ContainsKey(end) && start != end)
            return (null, double.PositiveInfinity);

        var path = new List<Airport>();
        var u = end;
        while (previous.ContainsKey(u))
        {
            path.Insert(0, u);
            u = previous[u];
        }
        path.Insert(0, start);

        Airport? nextHop = path.Count >= 2 ? path[1] : null;
        double totalHours = timeFromStart[end];

        return (nextHop, totalHours);
    }
}