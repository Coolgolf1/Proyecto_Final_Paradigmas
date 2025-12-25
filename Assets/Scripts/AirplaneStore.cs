using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AirplaneStore : MonoBehaviour
{
    private double _smallAirplanePriceMultiplier = 1;
    [SerializeField] private TMP_Text smallAirplaneTitle;
    [SerializeField] private Image smallAirplaneImage;
    [SerializeField] private TMP_Text smallAirplanePrice;
    [SerializeField] private TMP_Text smallAirplaneCapacity;
    [SerializeField] private TMP_Text smallAirplaneSpeed;
    [SerializeField] private TMP_Text smallAirplaneRange;
    [SerializeField] private TMP_Dropdown smallAirplaneSelector;
    [SerializeField] private Button smallAirplaneBuy;

    private double _mediumAirplanePriceMultiplier = 1;
    [SerializeField] private TMP_Text mediumAirplaneTitle;
    [SerializeField] private Image mediumAirplaneImage;
    [SerializeField] private TMP_Text mediumAirplanePrice;
    [SerializeField] private TMP_Text mediumAirplaneCapacity;
    [SerializeField] private TMP_Text mediumAirplaneSpeed;
    [SerializeField] private TMP_Text mediumAirplaneRange;
    [SerializeField] private TMP_Dropdown mediumAirplaneSelector;
    [SerializeField] private Button mediumAirplaneBuy;

    private double _largeAirplanePriceMultiplier = 1;
    [SerializeField] private TMP_Text largeAirplaneTitle;
    [SerializeField] private Image largeAirplaneImage;
    [SerializeField] private TMP_Text largeAirplanePrice;
    [SerializeField] private TMP_Text largeAirplaneCapacity;
    [SerializeField] private TMP_Text largeAirplaneSpeed;
    [SerializeField] private TMP_Text largeAirplaneRange;
    [SerializeField] private TMP_Dropdown largeAirplaneSelector;
    [SerializeField] private Button largeAirplaneBuy;

    [SerializeField] private GameObject earth;

    [SerializeField] private Button openStore;
    [SerializeField] private Button closeStore;

    [SerializeField] private GameObject buyMessagePanel;
    [SerializeField] private TMP_Text buyMessage;
    [SerializeField] private Button closeBuyMessage;

    public Dictionary<AirplaneTypes, List<Airplane>> AvailableAirplanes { get; private set; }

    private int smallAirplanes = 9;
    private int mediumAirplanes = 7;
    private int largeAirplanes = 5;

    private InfoSingleton _info = InfoSingleton.GetInstance();
    private AirplaneFactory _airplaneFactory = AirplaneFactory.GetInstance();
    private EconomyManager _economy = EconomyManager.GetInstance();

    public void smallAirplaneBought()
    {
        // If no more remaining cannot buy
        if (smallAirplanes == 0)
        {
            //BuyMessageFailureNoStock("small");
            return;
        }

        if (GameConstants.smallPrice * _smallAirplanePriceMultiplier > Player.Money)
        {
            //BuyMessageFailureNotEnoughCoins("small");
            return;
        }

        // Check valid index
        int index = smallAirplaneSelector.value;
        if (index > smallAirplaneSelector.options.Count)
            return;

        // Save airport name
        string airportName = smallAirplaneSelector.options[index].text;

        // Check airport exists
        if (!_info.savedAirports.ContainsKey(airportName))
            return;

        _economy.SubtractCoins((int)(GameConstants.smallPrice * _smallAirplanePriceMultiplier));

        // Create airplane
        Airplane airplane = (Airplane)_airplaneFactory.Build(AirplaneTypes.Small, earth.transform);
        _info.airplanes.Add(airplane);

        // Add airplane to hangar
        _info.savedAirports[airportName].Hangar.Add(airplane);

        // Remove airplane from remaining
        if (smallAirplanes > 0)
        {
            smallAirplanes--;
        }
        UpdateButtons();
        // Success message
        //BuyMessageSuccess("small", airportName);

        // Launch flight
        FlightLauncher.LaunchNewFlights();
    }

    public void mediumAirplaneBought()
    {
        // If no more remaining cannot buy
        if (mediumAirplanes == 0)
        {
            //BuyMessageFailureNoStock("medium");
            return;
        }

        if (GameConstants.mediumPrice * _mediumAirplanePriceMultiplier > Player.Money)
        {
            //BuyMessageFailureNotEnoughCoins("medium");
            return;
        }

        // Check valid index
        int index = mediumAirplaneSelector.value;
        if (index > mediumAirplaneSelector.options.Count)
            return;

        // Save airport name
        string airportName = mediumAirplaneSelector.options[index].text;

        // Check airport exists
        if (!_info.savedAirports.ContainsKey(airportName))
            return;

        _economy.SubtractCoins((int)(GameConstants.mediumPrice * _mediumAirplanePriceMultiplier));

        // Create airplane
        Airplane airplane = (Airplane)_airplaneFactory.Build(AirplaneTypes.Medium, earth.transform);
        _info.airplanes.Add(airplane);

        // Add airplane to hangar
        _info.savedAirports[airportName].Hangar.Add(airplane);

        // Remove airplane from remaining
        if (mediumAirplanes > 0)
        {
            mediumAirplanes--;
        }

        // Success message
        //BuyMessageSuccess("medium", airportName);
        UpdateButtons();
        // Launch flight
        FlightLauncher.LaunchNewFlights();
    }
    public void largeAirplaneBought()
    {
        // If no more remaining cannot buy
        if (largeAirplanes == 0)
        {
            //BuyMessageFailureNoStock("large");
            return;
        }

        if (GameConstants.largePrice * _largeAirplanePriceMultiplier > Player.Money)
        {
            //BuyMessageFailureNotEnoughCoins("large");
            return;
        }

        // Check valid index
        int index = largeAirplaneSelector.value;
        if (index > largeAirplaneSelector.options.Count)
            return;

        // Save airport name
        string airportName = largeAirplaneSelector.options[index].text;

        // Check airport exists
        if (!_info.savedAirports.ContainsKey(airportName))
            return;

        _economy.SubtractCoins((int)(GameConstants.largePrice * _largeAirplanePriceMultiplier));

        // Create airplane
        Airplane airplane = (Airplane)_airplaneFactory.Build(AirplaneTypes.Large, earth.transform);
        _info.airplanes.Add(airplane);

        // Add airplane to hangar
        _info.savedAirports[airportName].Hangar.Add(airplane);

        // Remove airplane from remaining
        if (largeAirplanes > 0)
        {
            largeAirplanes--;
        }

        // Success message
        //BuyMessageSuccess("large", airportName);
        UpdateButtons();
        // Launch flight
        FlightLauncher.LaunchNewFlights();

    }

    public void LoadStore()
    {
        double baseSpeed = GameConstants.relativeSpeed * GameConstants.speedMultiplier;

        // Small Airplane 
        smallAirplanePrice.text = $"Price: {GameConstants.smallPrice * _smallAirplanePriceMultiplier} coins";
        smallAirplaneCapacity.text = $"Capacity: {GameConstants.smallCapacity}";
        smallAirplaneSpeed.text = $"Speed: {(int)(baseSpeed * GameConstants.smallSpeedMultiplier * 5)}  km/h";
        smallAirplaneRange.text = $"Range: {GameConstants.smallRange} km";
        
        smallAirplaneBuy.onClick.AddListener(smallAirplaneBought);

        // Medium Airplane 
        mediumAirplanePrice.text = $"Price: {GameConstants.mediumPrice * _mediumAirplanePriceMultiplier} coins";
        mediumAirplaneCapacity.text = $"Capacity: {GameConstants.mediumCapacity}";
        mediumAirplaneSpeed.text = $"Speed: {(int)(baseSpeed * GameConstants.mediumSpeedMultiplier * 5)} km/h";
        mediumAirplaneRange.text = $"Range: {GameConstants.mediumRange} km";
        mediumAirplaneBuy.onClick.AddListener(mediumAirplaneBought);

        // Large Airplane 
        largeAirplanePrice.text = $"Price: {GameConstants.largePrice * _largeAirplanePriceMultiplier} coins";
        largeAirplaneCapacity.text = $"Capacity: {GameConstants.largeCapacity}";
        largeAirplaneSpeed.text = $"Speed: {(int)(baseSpeed * GameConstants.largeSpeedMultiplier * 5)} km/h";
        largeAirplaneRange.text = $"Range: {GameConstants.largeRange} km";
        largeAirplaneBuy.onClick.AddListener(largeAirplaneBought);

        // Dropdowns
        UpdateAirports();
        
    }

    private void UpdateAirports()
    {
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (Airport airport in Player.UnlockedAirports)
        {
            options.Add(new TMP_Dropdown.OptionData(airport.Name));
        }

        smallAirplaneSelector.ClearOptions();
        mediumAirplaneSelector.ClearOptions();
        largeAirplaneSelector.ClearOptions();

        smallAirplaneSelector.AddOptions(options);
        mediumAirplaneSelector.AddOptions(options);
        largeAirplaneSelector.AddOptions(options);
        UpdateButtons();

    }

    private void UpdateButtons()
    {
        if (_economy.GetBalance() >= GameConstants.smallPrice * _smallAirplanePriceMultiplier && smallAirplanes != 0)
        {
            smallAirplaneBuy.interactable = true;
        }
        else
        {
            smallAirplaneBuy.interactable = false;
        }


        if (_economy.GetBalance() >= GameConstants.mediumPrice * _mediumAirplanePriceMultiplier && mediumAirplanes != 0)
        {
            mediumAirplaneBuy.interactable = true;
        }
        else
        {
            mediumAirplaneBuy.interactable = false;
        }

        if (_economy.GetBalance() >= GameConstants.largePrice * _largeAirplanePriceMultiplier && largeAirplanes != 0)
        {
            largeAirplaneBuy.interactable = true;
        }
        else
        {
            largeAirplaneBuy.interactable = false;
        }
    }

    public void Awake()
    {
        UIEvents.LoadedListeners.AddListener(LoadStore);
        buyMessagePanel.gameObject.SetActive(false);

        openStore.onClick.AddListener(ShowStoreUI);
        closeStore.onClick.AddListener(CloseStoreUI);
        //closeBuyMessage.onClick.AddListener(CloseBuyMessage);

        //smallAirplaneBuy.onClick.AddListener(OpenBuyMessage);
        //mediumAirplaneBuy.onClick.AddListener(OpenBuyMessage);
        //largeAirplaneBuy.onClick.AddListener(OpenBuyMessage);

        UIEvents.OnRouteStoreEnter.AddListener(CloseStoreUI);

        gameObject.SetActive(false);
    }

    public void ShowStoreUI()
    {
        _economy.SetCoins(1000000);
        gameObject.SetActive(true);
        UpdateAirports();
        UIEvents.OnAirplaneStoreEnter.Invoke();
    }

    public void CloseStoreUI()
    {
        gameObject.SetActive(false);
        UIEvents.OnAirplaneStoreExit.Invoke();
    }

    //public void OpenBuyMessage()
    //{
    //    buyMessagePanel.gameObject.SetActive(true);

    //    // Disable close store button
    //    closeStore.interactable = false;
    //}

    //public void CloseBuyMessage()
    //{
    //    buyMessagePanel.gameObject.SetActive(false);

    //    // Enable close store button
    //    closeStore.interactable = true;
    //}

    //public void BuyMessageSuccess(string airplaneType, string airportName)
    //{
    //    buyMessage.text = $"Sucess!\n\nThe {airplaneType} airplane is now in {airportName}";
    //}

    //public void BuyMessageFailureNotEnoughCoins(string airplaneType)
    //{
    //    buyMessage.text = $"Not enough coins to buy {airplaneType} airplane!";
    //}

    //public void BuyMessageFailureNoStock(string airplaneType)
    //{
    //    buyMessage.text = $"You have already bought all {airplaneType} airplanes!";
    //}
}