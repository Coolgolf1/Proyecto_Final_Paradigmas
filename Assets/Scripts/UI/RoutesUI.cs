using System;
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
    private GameMaster _gm = GameMaster.GetInstance();

    private int _price;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        UIEvents.LoadedListeners.AddListener(LoadStore);
        buyRoute.onClick.AddListener(BuyRoute);
        openUI.onClick.AddListener(ShowStoreUI);
        closeUI.onClick.AddListener(CloseStoreUI);
        gameObject.SetActive(false);

        UIEvents.OnAirplaneStoreEnter.AddListener(CloseStoreUI);
        UIEvents.OnEndGameEnter.AddListener(CloseStoreUI);
        _economy.MoneyChange += HandleMoneyChange;
    }

    private void HandleMoneyChange(object sender, EventArgs e)
    {
        UpdatePriceAndButton();
    }
    void LoadStore()
    {

        UpdateFirstChoice();
        UpdateSecondChoice();
        UpdatePriceAndButton();

        airport2.onValueChanged.AddListener(delegate { UpdatePriceAndButton(); });
        airport1.onValueChanged.AddListener(delegate { UpdateSecondChoice(); UpdatePriceAndButton(); });

    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateFirstChoice()
    {
        List<TMP_Dropdown.OptionData> options1 = new List<TMP_Dropdown.OptionData>();

        foreach (Airport airport in Player.UnlockedAirports)
        {
            if (airport.Name != airport2.options[airport2.value].text)
                options1.Add(new TMP_Dropdown.OptionData(airport.Name));
        }

        airport1.ClearOptions();
        airport1.AddOptions(options1);
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

    void UpdatePriceAndButton()
    {
        string a1value = airport1.options[airport1.value].text;
        string a2value = airport2.options[airport2.value].text;

        Airport a1 = _info.savedAirports[a1value];
        Airport a2 = _info.savedAirports[a2value];

        double distance = Auxiliary.GetDirectDistanceBetweenAirports(a1, a2);

        _price = (int)Mathf.Pow((float)distance, 1.55f);


        if (_info.savedRoutes.ContainsKey($"{a1value}-{a2value}"))
        {
            buyRoute.interactable = true;
            priceText.text = $"+{(_price / 2).ToString("#,#")} coins";
            priceText.color = Color.darkGreen;
            buyRoute.GetComponentInChildren<TMP_Text>().text = "Remove";
            buyRoute.GetComponent<Image>().color = Color.red;
        }
        else if (_economy.GetBalance() < _price)
        {
            priceText.text = $"{_price.ToString("#,#")} coins";
            buyRoute.interactable = false;
            priceText.color = Color.black;
            buyRoute.GetComponentInChildren<TMP_Text>().text = "Buy";
            buyRoute.GetComponent<Image>().color = Color.white;
        }
        else
        {
            priceText.text = $"{_price.ToString("#,#")} coins";
            buyRoute.interactable = true;
            priceText.color = Color.black;
            buyRoute.GetComponentInChildren<TMP_Text>().text = "Buy";
            buyRoute.GetComponent<Image>().color = Color.white;
        }
    }

    public void ShowStoreUI()
    {
        //_economy.SetCoins(1000000);

        UpdateFirstChoice();
        UpdateSecondChoice();
        UpdatePriceAndButton();

        gameObject.SetActive(true);
        UIEvents.OnRouteStoreEnter.Invoke();
    }

    public void CloseStoreUI()
    {
        gameObject.SetActive(false);
        if (_gm.currentState != _gm.End)
            UIEvents.OnRouteStoreExit.Invoke();
    }

    void BuyRoute()
    {
        string location1 = airport1.options[airport1.value].text;
        string location2 = airport2.options[airport2.value].text;
        if (!_info.savedRoutes.ContainsKey($"{location1}-{location2}"))
        {
            if (_economy.SubtractCoins(_price))
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

                UpdatePriceAndButton();

                _info.notificationSystem.AddNotification($"Created Route {location1}-{location2}", "route", "blue");
            }
        }
        else
        {
            _economy.AddCoins(_price / 2);

            _info.savedRoutes[$"{location1}-{location2}"].AwaitingRemoval = true;

            _info.savedRoutes.Remove($"{location1}-{location2}");
            _info.savedRoutes.Remove($"{location2}-{location1}");
            Auxiliary.LoadRouteDistances(_info.savedRoutes);

            // Calculate initial Dijkstra Graph
            Auxiliary.CalculateDijkstraGraph();

            _info.notificationSystem.AddNotification($"Removed Route {location1}-{location2}", "route", "red");

            UpdatePriceAndButton();
        }
    }


}
