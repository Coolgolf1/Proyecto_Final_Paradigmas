using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AirplaneStore : MonoBehaviour
{
    public Dictionary<AirplaneTypes, List<Airplane>> AvailableAirplanes { get; private set; }
    public GameObject Earth { get; private set; }

    private AirplaneFactory _airplaneFactory = AirplaneFactory.GetInstance();

    private int smallAirplanes = 9;
    private int mediumAirplanes = 7;
    private int largeAirplanes = 5;


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

    public void Start()
    {
        Transform smallAirplane = GameObject.Find("SmallAirplane").transform;
        Transform child = smallAirplane.GetChild(0);
        Debug.Log(child.name);
    }
}