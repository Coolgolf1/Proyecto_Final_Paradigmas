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
        List<Airport> destinations = Player.UnlockedAirports.ToList();

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

                    HashSet<Airplane> fullAirplanes = new HashSet<Airplane>();

                    while (_info.savedAirports[origAirport.Name].TravellersToAirport[objAirport] > 0)
                    {
                        Flight flight;

                        (Airplane objAirplane, Airport nextHop) = _info.savedAirports[origAirport.Name].FindHopForTravellersToAirport(objAirport);

                        if (objAirplane is null || nextHop is null)
                        {
                            break;
                        }

                        if (fullAirplanes.Contains(objAirplane))
                            break;

                        if (createdFlights.Keys.Contains(objAirplane))
                        {
                            flight = createdFlights[objAirplane];
                        }
                        //else if (airplaneInFlight.Contains(objAirplane))
                        //    continue;
                        else
                        {
                            GameObject flightGO = new GameObject();
                            flightGO.name = $"{origAirport.Name}-{nextHop.Name}";
                            flight = flightGO.AddComponent<Flight>();

                            flight.Initialise(origAirport, nextHop, _info.savedRoutes[$"{origAirport.Name}-{nextHop.Name}"], objAirplane);
                            _info.flights.Add(flight);
                            createdFlights[objAirplane] = flight;

                            //airplaneInFlight.Add(objAirplane);
                        }

                        flight.Embark(_info.savedAirports[origAirport.Name].TravellersToAirport[objAirport], objAirport);

                        if (flight.Full)
                            fullAirplanes.Add(objAirplane);
                    }
                }
            }
        }
        foreach (Flight tempFlight in createdFlights.Values)
        {
            tempFlight.StartFlight();
        }

    }

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
}
