using System;
using UnityEngine;

public class Init : MonoBehaviour
{
    private InfoSingleton _info = InfoSingleton.GetInstance();

    public void SaveDataOfAirports(GameObject airportPrefab, Transform earthTransform)
    {
        foreach (string city in _info.locations.Keys)
        {
            // Create Airport GameObject
            GameObject airportGO = Instantiate(airportPrefab, earthTransform);
            airportGO.name = city;

            // Get components of GameObject
            Airport airport = airportGO.GetComponent<Airport>();
            Location location = airportGO.GetComponentInChildren<Location>();

            // Save location and airport properties
            location.Initialise(id: city, name: $"{city}_Loc", coords: _info.locations[city]);
            airport.Initialise(id: _info.stringCityCodes[city], name: city, location: location);

            // Save airport 
            _info.savedAirports[city] = airport;
        }
    }

    public void SaveDataOfRoutes(GameObject routePrefab, Transform earthTransform)
    {

        foreach (Tuple<string, string> routeTuple in _info.stringCityRoutes)
        {
            // Create Route GameObject
            GameObject routeGO = Instantiate(routePrefab, earthTransform);
            routeGO.name = $"{routeTuple.Item1}-{routeTuple.Item2}";

            // Get route component of GameObject
            Route route = routeGO.GetComponent<Route>();

            // Initialise route 
            route.Initialise(airport1: _info.savedAirports[routeTuple.Item1], airport2: _info.savedAirports[routeTuple.Item2]);

            // Save route in both ways
            _info.savedRoutes[routeGO.name] = route;
            _info.savedRoutes[$"{routeTuple.Item2}-{routeTuple.Item1}"] = route;
        }
    }

    public void InitTravellersInAirports()
    {
        foreach (Airport airport in _info.savedAirports.Values)
        {
            airport.InitTravellers();
        }
    }
}
