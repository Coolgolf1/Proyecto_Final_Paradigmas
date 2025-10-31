using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private GameObject locationPrefab;
    [SerializeField]
    private GameObject earth;

    private Dictionary<string, Vector3> locations = new Dictionary<string, Vector3>()
    {
        { "Madrid", new Vector3(11.2700005f, 14.1899996f, -8.56000042f) },
        { "San Francisco", new Vector3(-14.2299995f,14.0200005f,-2.3499999f) },
        { "Shanghai", new Vector3(3.6400001f,7.21999979f,18.3999996f) },
        { "Paris", new Vector3(11.1300001f,15.6400003f,-5.75f ) },
        { "Dubai", new Vector3(18.3099995f,6.26000023f,5.48000002f) }
    };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Location madridLoc;
        Location dubaiLoc;

        var locationObject = Instantiate(locationPrefab, earth.transform);
        Location locationComp = locationObject.AddComponent<Location>();
        locationComp.id = "Madrid";
        locationComp.coords = locations["Madrid"];

        madridLoc = locationComp;

        var locationObject2 = Instantiate(locationPrefab, earth.transform);
        Location locationComp2 = locationObject2.AddComponent<Location>();
        locationComp2.id = "Dubai";
        locationComp2.coords = locations["Dubai"];

        dubaiLoc = locationComp2;




        //foreach (string city in locations.Keys)
        //{
        //    var locationObject = Instantiate(locationPrefab, earth.transform);
        //    Location locationComp = locationObject.AddComponent<Location>();
        //    locationComp.id = city;
        //    locationComp.coords = locations[city];
        //    if (city == "Madrid")
        //    {
        //        madridLoc = locationComp;
        //    } else if (city == "Dubai")
        //    {
        //        dubaiLoc = locationComp;
        //    }
        //}

        GameObject madrid = new GameObject();
        var madridAirport = madrid.AddComponent<Airport>();
        GameObject dubai = new GameObject();
        var dubaiAirport = dubai.AddComponent<Airport>();

        

        madridAirport.location = madridLoc;
        dubaiAirport.location = dubaiLoc;

        var ruta = new GameObject();
        Route rutaComp = ruta.AddComponent<Route>();
        rutaComp.airport1 = madridAirport;
        rutaComp.airport2 = dubaiAirport;


        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
