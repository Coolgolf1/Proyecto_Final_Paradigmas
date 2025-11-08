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

    private static (Airplane, double) GetFastestAirplaneAndTime(Airport origin, Edge edge, Airport start)
    {
        // Compute the shortest possible time (in hours) among all airplanes
        double bestDistance = double.PositiveInfinity;
        Airplane bestAirplane = null;
        //double flightTime = 0;

        foreach (Airplane airplane in start.hangar)
        {
            // Check if it is in another hangar different to the ones in route
            Airport airportHangar = Info.GetAirportOfAirplane(airplane);

            //if (airportHangar != start && airportHangar != edge.To)
            //{
            //    continue;
            //}

            //if (airportHangar != start)
            //{
            //    continue;
            //}

            double tempDistance;

            Flight flight = Info.GetFlightOfAirplane(airplane);

            Debug.Log(airplane);

            // 1. Flight assigned but in Hangar
            if (flight is not null)
            {
                Airport airportDest = flight.airportDest;

                if (airportDest == edge.To)
                {
                    tempDistance = 0;
                }
                else
                {
                    tempDistance = flight.route.distance;

                }
            }
            // 2. Flight not yet created
            else
            {
                tempDistance = Auxiliary.GetDistanceBeteweenAirports(origin, edge.To);
            }

            // Choose best airplane with distance
            if (tempDistance <= bestDistance)
            {
                bestDistance = tempDistance;
                bestAirplane = airplane;
            }



            // Calcular Ruta Más Corta y Barata

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

        return (bestAirplane, bestDistance);
    }

    //public static (Airplane) GetAirplaneInHangar()
    //{
    //    
    //}

    public static Airport GetNextHop(List<Airport> path)
    {
        return path.Count >= 2 ? path[1] : null;
    }

    public static (Airplane, List<Airport>) Dijkstra(
          Dictionary<Airport, List<Edge>> graph,
          Airport start,
          Airport end, bool getFullPath = false)
    {
        Dictionary<Airport, double> distanceFromStart = new Dictionary<Airport, double>();
        Dictionary<Airport, Airport> previous = new Dictionary<Airport, Airport>();
        PriorityQueue<Airport> queue = new PriorityQueue<Airport>();
        Dictionary<Airport, Airplane> airplaneUsed = new Dictionary<Airport, Airplane>();

        // Initialize 
        foreach (Airport node in graph.Keys)
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

            if (!graph.ContainsKey(current))
                continue;

            if (processed.Contains(current))
                continue;

            processed.Add(current);

            //Debug.Log("===============");
            foreach (Edge edge in graph[current])
            {
                (Airplane airplane, double cost) = GetFastestAirplaneAndTime(current, edge, start);

                if (airplane is null) continue;
                if (double.IsInfinity(cost)) continue;

                Airport neighbor = edge.To;
                double newDistance = currentDistance + cost;

                //Debug.Log(newDistance);
                //Debug.Log(distanceFromStart[neighbor]);

                //Debug.Log(edge.To);
                //Debug.Log(distanceFromStart[neighbor]);
                if (newDistance <= distanceFromStart[neighbor])
                {
                    //Debug.Log(neighbor);
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
            //Debug.Log("HEEEEEELPPPPPPPPPPPPPP");
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