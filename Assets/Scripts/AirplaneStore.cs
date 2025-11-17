using System;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneStore
{
    public Dictionary<AirplaneTypes, List<Airplane>> AvailableAirplanes { get; private set; }
    public GameObject Earth { get; private set; }

    private AirplaneFactory _airplaneFactory = AirplaneFactory.GetInstance();

    private int smallAirplanes = 0;
    private int mediumAirplanes = 0;
    private int largeAirplanes = 0;


    public AirplaneStore(GameObject earth, int numberOfSmallAirplanes, int numberOfMediumAirplanes, int numberOfLargeAirplanes)
    {
        Earth = earth;
        smallAirplanes = numberOfSmallAirplanes;
        mediumAirplanes = numberOfMediumAirplanes;
        largeAirplanes = numberOfLargeAirplanes;

        Initialise();
    }

    private void Initialise()
    {
        foreach (AirplaneTypes type in Enum.GetValues(typeof(AirplaneTypes)))
        {
            AvailableAirplanes.Add(type, new List<Airplane>());
        }

        for (int i = 0; i < smallAirplanes; i++)
        {
            Airplane smallAirplane = (Airplane)_airplaneFactory.Build(AirplaneTypes.Small, Earth.transform);
            AvailableAirplanes[AirplaneTypes.Small].Add(smallAirplane);
        }

        for (int i = 0; i < mediumAirplanes; i++)
        {
            Airplane mediumAirplane = (Airplane)_airplaneFactory.Build(AirplaneTypes.Medium, Earth.transform);
            AvailableAirplanes[AirplaneTypes.Medium].Add(mediumAirplane);
        }

        for (int i = 0; i < largeAirplanes; i++)
        {
            Airplane largeAirplane = (Airplane)_airplaneFactory.Build(AirplaneTypes.Large, Earth.transform);
            AvailableAirplanes[AirplaneTypes.Large].Add(largeAirplane);
        }
    }
}