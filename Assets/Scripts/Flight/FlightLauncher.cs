using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FlightLauncher
{
    private static InfoSingleton _info = InfoSingleton.GetInstance();

    public static void LaunchFlights()
    {
        List<Airport> destinations = _info.savedAirports.Values.ToList();

        Dictionary<Airport, Dictionary<Airplane, Flight>> airportFlights = new Dictionary<Airport, Dictionary<Airplane, Flight>>();
        foreach (Airport airport in destinations)
        {
            airportFlights[airport] = new Dictionary<Airplane, Flight>();
        }

        Dictionary<Airplane, Flight> createdFlights = new Dictionary<Airplane, Flight>();

        // For all travellers in origin airport, assign each of the travellers an airplane
        Queue<Airport> airportQueue = new Queue<Airport>(destinations);

        List<Airplane> airplaneInFlight = new List<Airplane>();

        while (airportQueue.Count > 0)
        {
            Airport origAirport = airportQueue.Dequeue();

            foreach (Airport objAirport in destinations)
            {
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

                    if (usedThisIteration.Contains(objAirplane))
                    {
                        break; // no more airplanes left
                    }
                    usedThisIteration.Add(objAirplane);

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

        foreach (Flight tempFlight in createdFlights.Values)
        {
            tempFlight.StartFlight();
        }
    }
}
