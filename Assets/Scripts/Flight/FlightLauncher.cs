using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FlightLauncher
{
    private static InfoSingleton _info = InfoSingleton.GetInstance();

    public static void InitFlightLauncher()
    {
        GameEvents.OnPlaneLandedAndBoarded.AddListener(LaunchAirplanesInHangar);
    }

    public static void LaunchNewFlights()
    {
        List<Airport> destinations = _info.savedAirports.Values.ToList();

        Dictionary<Airplane, Flight> createdFlights = new Dictionary<Airplane, Flight>();

        // For all travellers in origin airport, assign each of the travellers an airplane
        Queue<Airport> airportQueue = new Queue<Airport>(destinations);



        List<Airplane> airplaneInFlight = new List<Airplane>();

        while (airportQueue.Count > 0)
        {
            Airport origAirport = airportQueue.Dequeue();

            PriorityQueue<Airport> airportDestinationQueue = new PriorityQueue<Airport>();

            foreach (Airport airport in origAirport.TravellersToAirport.Keys)
            {
                airportDestinationQueue.Enqueue(airport, -origAirport.TravellersToAirport[airport]);
            }

            while (airportDestinationQueue.Count > 0)
            {
                {
                    Airport objAirport = airportDestinationQueue.Dequeue();
                    if (origAirport == objAirport)
                    {
                        continue;
                    }

                    if (_info.savedAirports[origAirport.Name].TravellersToAirport[objAirport] <= 0)
                    {
                        continue;
                    }

                    HashSet<Airplane> usedThisIteration = new HashSet<Airplane>();

                    while (_info.savedAirports[origAirport.Name].TravellersToAirport[objAirport] > 0)
                    {
                        Flight flight;

                        (Airplane objAirplane, Airport nextHop) = _info.savedAirports[origAirport.Name].FindHopForTravellersToAirport(objAirport);

                        if (objAirplane is null || nextHop is null)
                        {
                            break;
                        }

                        //if (usedThisIteration.Contains(objAirplane))
                        //{
                        //    break; // no more airplanes left
                        //}
                        //usedThisIteration.Add(objAirplane);

                        if (createdFlights.Keys.Contains(objAirplane))
                        {
                            flight = createdFlights[objAirplane];
                        }
                        else if (airplaneInFlight.Contains(objAirplane))
                            continue;
                        else
                        {
                            GameObject flightGO = new GameObject();
                            flightGO.name = $"{origAirport.Name}-{nextHop.Name}";
                            flight = flightGO.AddComponent<Flight>();

                            flight.Initialise(origAirport, nextHop, _info.savedRoutes[$"{origAirport.Name}-{nextHop.Name}"], objAirplane);
                            _info.flights.Add(flight);
                            createdFlights[objAirplane] = flight;

                            airplaneInFlight.Add(objAirplane);
                        }

                        flight.Embark(_info.savedAirports[origAirport.Name].TravellersToAirport[objAirport], objAirport);
                    }
                }
            }
        }
        foreach (Flight tempFlight in createdFlights.Values)
        {
            tempFlight.StartFlight();
        }

    }

    //public static void LaunchEmptyFlights()
    //{
    //    // Save airplanes that are in any hangar in the world
    //    List<Airplane> airplanesInHangar = new List<Airplane>();
    //    foreach (Airport airport in _info.savedAirports.Values)
    //    {
    //        airplanesInHangar.AddRange(airport.Hangar);
    //    }

    //    List<Flight> createdFlights = new List<Flight>();
    //    foreach (Airplane airplane in airplanesInHangar)
    //    { 
    //        // Check that the airplane has no flight assigned yet
    //        Flight flightOfAirplane = _info.GetFlightOfAirplane(airplane);
    //        if (flightOfAirplane != null)
    //            continue;

    //        // Get reachable airports for this airplane
    //        Airport airportOfAirplane = _info.GetAirportOfAirplane(airplane);
    //        List<Airport> reachableAirports = airportOfAirplane.GetReachableAirportsForAirplane(airplane);

    //        foreach (Airport reachableAirport in reachableAirports)
    //        {
    //            // If no passengers to airport, skip it
    //            int passengersInAirport = airportOfAirplane.TravellersToAirport[reachableAirport];
    //            if (passengersInAirport <= 0)
    //                continue;

    //            // Create flight to airport
    //            (Airplane _, List<Airport> path) = RouteAssigner.Dijkstra(airportOfAirplane, reachableAirport);
    //            Airport hop = RouteAssigner.GetNextHop(path);

    //            if (hop is not null)
    //            {
    //                Flight emptyFlight;

    //                GameObject flightGO = new GameObject();
    //                flightGO.name = $"{airportOfAirplane.Name}-{reachableAirport.Name}";
    //                emptyFlight = flightGO.AddComponent<Flight>();
    //                emptyFlight.Initialise(airportOfAirplane, hop, _info.savedRoutes[$"{airportOfAirplane.Name}-{reachableAirport.Name}"], airplane);
    //                _info.flights.Add(emptyFlight);
    //                createdFlights.Add(emptyFlight);

    //                _info.airplanesGoingFromEmptyAirport[airportOfAirplane].Add(airplane);
    //                break;
    //            }

    //            // Check another layer if not created a flight
    //            List<Airport> reachableAirportsFromAirport = reachableAirport.GetReachableAirportsForAirplane(airplane);
    //            foreach (Airport reachableAirportFromAirport in reachableAirportsFromAirport)
    //            {
    //                // If no passengers to airport, skip it
    //                passengersInAirport = reachableAirport.TravellersToAirport[reachableAirportFromAirport];
    //                if (passengersInAirport <= 0)
    //                    continue;

    //                // Create flight to airport
    //                (Airplane _, List<Airport> pathFarther) = RouteAssigner.Dijkstra(airportOfAirplane, reachableAirportFromAirport);
    //                hop = RouteAssigner.GetNextHop(pathFarther);

    //                if (hop is not null)
    //                {
    //                    Flight emptyFlight;

    //                    GameObject flightGO = new GameObject();
    //                    flightGO.name = $"{airportOfAirplane.Name}-{reachableAirportFromAirport.Name}";
    //                    emptyFlight = flightGO.AddComponent<Flight>();
    //                    emptyFlight.Initialise(airportOfAirplane, hop, _info.savedRoutes[$"{airportOfAirplane.Name}-{reachableAirportFromAirport.Name}"], airplane);
    //                    _info.flights.Add(emptyFlight);
    //                    createdFlights.Add(emptyFlight);

    //                    _info.airplanesGoingFromEmptyAirport[airportOfAirplane].Add(airplane);
    //                    break;
    //                }
    //            }
    //        }
    //    }

    //    foreach (Flight flight in createdFlights)
    //    {
    //        flight.StartFlight();
    //    }
    //}
    //public static void LaunchEmptyFlights()
    //{
    //    List<Airplane> airplanesInHangar = new List<Airplane>();
    //    foreach (Airport airport in _info.savedAirports.Values)
    //    {
    //        airplanesInHangar.AddRange(airport.Hangar);
    //    }

    //    Dictionary<Airport, int> incomingCapacity = new Dictionary<Airport, int>();

    //    // Pre-fill incoming capacity from existing flights
    //    foreach (Flight f in _info.flights)
    //    {
    //        // Only count flights that are NOT full (or explicitly empty)
    //        if (!f.Full)
    //        {
    //            if (!incomingCapacity.ContainsKey(f.AirportDest)) incomingCapacity[f.AirportDest] = 0;
    //            incomingCapacity[f.AirportDest] += f.Airplane.Capacity;
    //        }
    //    }

    //    List<Flight> createdFlights = new List<Flight>();

    //    foreach (Airplane airplane in airplanesInHangar)
    //    {
    //        if (_info.GetFlightOfAirplane(airplane) != null) continue;

    //        Airport origin = _info.GetAirportOfAirplane(airplane);

    //        List<Airport> reachableAirports = origin.GetReachableAirportsForAirplane(airplane);

    //        // --- FIX 2: PRIORITIZE BUSIEST AIRPORTS ---
    //        // Don't just take the first airport in the list. Sort by most desperate.
    //        // We order by Total Passengers descending.
    //        var sortedAirports = reachableAirports.OrderByDescending(a => GetTotalPassengers(a)).ToList();

    //        foreach (Airport targetAirport in sortedAirports)
    //        {
    //            // Now we check if we should send it
    //            if (ShouldSendPlane(targetAirport, airplane, incomingCapacity))
    //            {
    //                Flight newFlight = CreateAndRegisterFlight(airplane, origin, targetAirport);
    //                if (newFlight != null)
    //                {
    //                    createdFlights.Add(newFlight);
    //                    break; // Plane assigned, move to next plane
    //                }
    //            }
    //        }
    //    }

    //    foreach (Flight flight in createdFlights)
    //    {
    //        flight.StartFlight();
    //    }
    //}

    public static void LaunchAirplanesInHangar()
    {
        foreach (Airport airport in _info.savedAirports.Values)
        {
            int totalTravellers = airport.TravellersToAirport.Values.Sum();

            Airport objAirport = airport.TravellersToAirport.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

            if (totalTravellers <= 0)
                continue;

            List<Airplane> airplanesInHangar = airport.Hangar.ToList();

            foreach (Airplane airplane in airplanesInHangar)
            {
                totalTravellers = airport.TravellersToAirport.Values.Sum();

                objAirport = airport.TravellersToAirport.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

                if (totalTravellers > 0)
                {
                    Flight flight;

                    (Airplane objAirplane, Airport nextHop) = _info.savedAirports[airport.Name].FindHopForTravellersToAirport(objAirport);

                    if (objAirplane is null || nextHop is null)
                    {
                        break;
                    }

                    if (_info.GetFlightOfAirplane(objAirplane) is not null)
                        continue;

                    GameObject flightGO = new GameObject();
                    flightGO.name = $"{airport.Name}-{nextHop.Name}";
                    flight = flightGO.AddComponent<Flight>();

                    flight.Initialise(airport, nextHop, _info.savedRoutes[$"{airport.Name}-{nextHop.Name}"], objAirplane);
                    _info.flights.Add(flight);

                    totalTravellers -= _info.savedAirports[airport.Name].TravellersToAirport[objAirport];

                    flight.Embark(_info.savedAirports[airport.Name].TravellersToAirport[objAirport], objAirport);

                    flight.StartFlight();
                }
                else
                {
                    break;
                }
            }
        }
    }

    // Helper to clean up the code
    private static int GetTotalPassengers(Airport airport)
    {
        int total = 0;
        // Check for null to be safe
        if (airport.TravellersToAirport != null)
        {
            foreach (var count in airport.TravellersToAirport.Values)
            {
                total += count;
            }
        }
        return total;
    }

    private static bool ShouldSendPlane(Airport target, Airplane plane, Dictionary<Airport, int> incomingCap)
    {
        int totalWaiting = GetTotalPassengers(target);

        if (totalWaiting <= 0) return false;

        int seatsOnTheWay = 0;
        if (incomingCap.ContainsKey(target))
        {
            seatsOnTheWay = incomingCap[target];
        }

        // Only send if demand is higher than current supply
        if (totalWaiting > seatsOnTheWay)
        {
            if (!incomingCap.ContainsKey(target)) incomingCap[target] = 0;
            incomingCap[target] += plane.Capacity;
            return true;
        }

        return false;
    }

    // (CreateAndRegisterFlight method remains the same as previous step)

    private static Flight CreateAndRegisterFlight(Airplane airplane, Airport from, Airport to)
    {
        (Airplane _, List<Airport> path) = RouteAssigner.Dijkstra(from, to);
        if (path is null)
        {
            return null;
        }
        Airport hop = RouteAssigner.GetNextHop(path);

        if (hop == null) return null;

        string routeKey = $"{from.Name}-{to.Name}";

        // Safety check
        if (!_info.savedRoutes.ContainsKey(routeKey))
        {
            return null;
        }

        GameObject flightGO = new GameObject();
        flightGO.name = $"{from.Name}-{to.Name}";
        Flight emptyFlight = flightGO.AddComponent<Flight>();

        emptyFlight.Initialise(from, hop, _info.savedRoutes[routeKey], airplane);

        _info.flights.Add(emptyFlight);

        if (!_info.airplanesGoingFromEmptyAirport.ContainsKey(from))
            _info.airplanesGoingFromEmptyAirport[from] = new List<Airplane>();

        _info.airplanesGoingFromEmptyAirport[from].Add(airplane);

        return emptyFlight;
    }
}
