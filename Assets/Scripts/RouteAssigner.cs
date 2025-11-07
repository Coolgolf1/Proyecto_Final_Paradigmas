using System.Collections.Generic;
using UnityEngine;

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
        //double flightTime = 0;

        foreach (Airplane airplane in Info.airplanes)
        {
            // Check if it is in another hangar different to the ones in route
            Airport airportHangar = Info.GetAirportOfAirplane(airplane);
            if (airportHangar != origin && airportHangar != edge.To)
            {
                continue;
            }

            if (airportHangar != origin)
            {
                continue;
            }
            else
            {
                Flight flight = Info.GetFlightOfAirplane(airplane);

                // 1. Flight assigned but in Hangar
                if (flight is not null)
                {
                    Airport airportDest = flight.airportDest;

                    if (airportDest == edge.To)
                    {
                    
                        return (airplane, 0);
                    }
                    else
                    {
                        continue;
                    }
                }
                // 2. Flight not yet created
                else
                {
                    return (airplane, 0);
                }

                // Calcular Ruta Más Corta y Barata
            }

            //// Check if the airplane is in possible route
            //if (Info.GetTakeoffAirportOfAirplane(airplane) != origin && Info.GetLandingAirportOfAirplane(airplane) != origin)
            //{
            //    continue;
            //}

            //// Get Takeoff Airport if in Flight
            //Airport takeoffAirport = Info.GetTakeoffAirportOfAirplane(airplane);

            //// Not in Flight
            //if (takeoffAirport == null)
            //{
            //    if (Info.GetAirportOfAirplane(airplane) == origin)
            //    {
            //        return (airplane, 0);
            //    }
            //    else if (Info.GetAirportOfAirplane(airplane) == edge.To)
            //    {
            //        flightTime = edge.distance / airplane.Speed;

            //        if (flightTime < bestTime)
            //        {
            //            bestTime = flightTime;
            //            bestAirplane = airplane;
            //        }
            //    }
            //    else
            //    {
            //        continue;
            //    }
            //}
            //// In Flight
            //else
            //{
            //    double distance = 0;

            //    // Flying away from origin airport
            //    if (takeoffAirport == origin)
            //    {
            //        // Add distance of coming back to origin airport
            //        distance += edge.distance;

            //        // Add remaining distance to destination airport
            //        distance += (1 - Info.GetFlightOfAirplane(airplane).FlightProgress) * edge.distance;
            //    }
            //    else
            //    {
            //        // Add remaining distance to destination airport
            //        distance += (1 - Info.GetFlightOfAirplane(airplane).FlightProgress) * edge.distance;
            //    }

            //    flightTime = distance / airplane.Speed;

            //    if (flightTime < bestTime)
            //    {
            //        bestTime = flightTime;
            //        bestAirplane = airplane;
            //    }
            //}
        }

        return (bestAirplane, bestTime);
    }

    //public static (Airplane) GetAirplaneInHangar()
    //{
    //    
    //}

    public static (Airplane, Airport) Dijkstra(
          Dictionary<Airport, List<Edge>> graph,
          Airport start,
          Airport end)
    {
        Dictionary<Airport, double> timeFromStart = new Dictionary<Airport, double>();
        Dictionary<Airport, Airport> previous = new Dictionary<Airport, Airport>();
        List<Airport> queue = new List<Airport>(graph.Keys);
        Dictionary<Airport, Airplane> airplaneUsed = new Dictionary<Airport, Airplane>();

        // Initialize 
        foreach (Airport node in graph.Keys)
            timeFromStart[node] = double.PositiveInfinity;

        timeFromStart[start] = 0;

        while (queue.Count > 0)
        {
            queue.Sort((a, b) => timeFromStart[a].CompareTo(timeFromStart[b]));
            Airport current = queue[0];
            queue.RemoveAt(0);

            if (current == start)
            {
                continue;
            }

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

            //if (current == end)
            //    break;

        }

        // If nothing found
        if (!previous.ContainsKey(end) && start != end)
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

        Airport nextHop = path.Count >= 2 ? path[1] : null;

        Airplane nextAirplane = null;
        if (nextHop != null)
            airplaneUsed.TryGetValue(nextHop, out nextAirplane);

        return (nextAirplane, nextHop);
    }
}