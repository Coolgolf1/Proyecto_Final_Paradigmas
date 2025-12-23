using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoutesUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Dropdown airport1;
    [SerializeField] private TMP_Dropdown airport2;
    [SerializeField] private Button buyRoute;
    [SerializeField] private Button openUI;
    [SerializeField] private Button closeUI;
    [SerializeField] private TMP_Text priceText;

    [Header("Routes")]
    [SerializeField] private GameObject routePrefab;
    [SerializeField] private GameObject earth;

    private EconomyManager _economy = EconomyManager.GetInstance();
    private InfoSingleton _info = InfoSingleton.GetInstance();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        UIEvents.LoadedListeners.AddListener(LoadStore);
        buyRoute.onClick.AddListener(BuyRoute);
        openUI.onClick.AddListener(ShowStoreUI);
        closeUI.onClick.AddListener(CloseStoreUI);
        gameObject.SetActive(false);

    }

    void LoadStore()
    {
        List<TMP_Dropdown.OptionData> options1 = new List<TMP_Dropdown.OptionData>();

        foreach (Airport airport in Player.UnlockedAirports)
        {
            options1.Add(new TMP_Dropdown.OptionData(airport.Name));
        }

        airport1.ClearOptions();
        airport1.AddOptions(options1);

        UpdateSecondChoice();

        airport1.onValueChanged.AddListener(delegate { UpdateSecondChoice(); });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateSecondChoice()
    {
        List<TMP_Dropdown.OptionData> options2 = new List<TMP_Dropdown.OptionData>();
        airport2.interactable = true;

        foreach (Airport airport in Player.UnlockedAirports)
        {
            if (airport.Name != airport1.options[airport1.value].text)
            {
                options2.Add(new TMP_Dropdown.OptionData(airport.Name));
            }
        }

        airport2.ClearOptions();
        airport2.AddOptions(options2);
    }

    public void ShowStoreUI()
    {
        _economy.SetCoins(1000000);
        gameObject.SetActive(true);
        UIEvents.OnStoreEnter.Invoke();
    }

    public void CloseStoreUI()
    {
        gameObject.SetActive(false);
        UIEvents.OnStoreExit.Invoke();
    }

    void BuyRoute()
    {
        string location1 = airport1.options[airport1.value].text;
        string location2 = airport2.options[airport2.value].text;
        if (!_info.savedRoutes.ContainsKey($"{location1}-{location2}"))
        {
            GameObject routeGO = Instantiate(routePrefab, earth.transform);

            routeGO.name = $"{location1}-{location2}";

            // Get route component of GameObject
            Route route = routeGO.GetComponent<Route>();

            // Initialise route
            route.Initialise(airport1: _info.savedAirports[location1], airport2: _info.savedAirports[location2]);

            // Save route in both ways
            _info.savedRoutes[routeGO.name] = route;
            _info.savedRoutes[$"{location2}-{location1}"] = route;

            Auxiliary.LoadRouteDistances(_info.savedRoutes);

            // Calculate initial Dijkstra Graph
            Auxiliary.CalculateDijkstraGraph();

            FlightLauncher.LaunchNewFlights();
        }
    }

   
}
