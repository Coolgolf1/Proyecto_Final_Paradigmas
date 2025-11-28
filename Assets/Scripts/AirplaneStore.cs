using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class AirplaneStore : MonoBehaviour 
{
    [SerializeField] private TMP_Text smallAirplaneTitle;
    [SerializeField] private Image smallAirplaneImage;
    [SerializeField] private TMP_Text smallAirplanePrice;
    [SerializeField] private TMP_Text smallAirplaneCapacity;
    [SerializeField] private TMP_Text smallAirplaneSpeed;
    [SerializeField] private TMP_Text smallAirplaneRange;
    [SerializeField] private TMP_Dropdown smallAirplaneSelector;
    [SerializeField] private Button smallAirplaneBuy;

    [SerializeField] private TMP_Text mediumAirplaneTitle;
    [SerializeField] private Image mediumAirplaneImage;
    [SerializeField] private TMP_Text mediumAirplanePrice;
    [SerializeField] private TMP_Text mediumAirplaneCapacity;
    [SerializeField] private TMP_Text mediumAirplaneSpeed;
    [SerializeField] private TMP_Text mediumAirplaneRange;
    [SerializeField] private TMP_Dropdown mediumAirplaneSelector;
    [SerializeField] private Button mediumAirplaneBuy;

    [SerializeField] private TMP_Text largeAirplaneTitle;
    [SerializeField] private Image largeAirplaneImage;
    [SerializeField] private TMP_Text largeAirplanePrice;
    [SerializeField] private TMP_Text largeAirplaneCapacity;
    [SerializeField] private TMP_Text largeAirplaneSpeed;
    [SerializeField] private TMP_Text largeAirplaneRange;
    [SerializeField] private TMP_Dropdown largeAirplaneSelector;
    [SerializeField] private Button largeAirplaneBuy;

    [SerializeField] private GameObject earth;

    public Dictionary<AirplaneTypes, List<Airplane>> AvailableAirplanes { get; private set; }

    private int smallAirplanes = 9;
    private int mediumAirplanes = 7;
    private int largeAirplanes = 5;

    private InfoSingleton _info = InfoSingleton.GetInstance();

    public void smallAirplaneBought()
    {
        int index = smallAirplaneSelector.value;
        if (index > smallAirplaneSelector.options.Count)
            return;

        string airportName = smallAirplaneSelector.options[index].text;

        if (!_info.savedAirports.ContainsKey(airportName))
            return;

        if (smallAirplanes > 0)
        {
            smallAirplanes--;
        }

        if (smallAirplanes == 0)
            smallAirplaneBuy.onClick.RemoveListener(smallAirplaneBought);
    }

    public void mediumAirplaneBought()
    {
        int index = mediumAirplaneSelector.value;
        if (index > mediumAirplaneSelector.options.Count)
            return;

        string airportName = mediumAirplaneSelector.options[index].text;

        if (!_info.savedAirports.ContainsKey(airportName))
            return;

        if (mediumAirplanes > 0)
        {
            mediumAirplanes--;
        }

        if (mediumAirplanes == 0)
            mediumAirplaneBuy.onClick.RemoveListener(mediumAirplaneBought);
    }
    public void largeAirplaneBought()
    {
        int index = largeAirplaneSelector.value;
        if (index > largeAirplaneSelector.options.Count)
            return;

        string airportName = largeAirplaneSelector.options[index].text;

        if (!_info.savedAirports.ContainsKey(airportName))
            return;

        if (largeAirplanes > 0)
        {
            largeAirplanes--;
        }

        if (largeAirplanes == 0)
            largeAirplaneBuy.onClick.RemoveListener(largeAirplaneBought);
    }

    public void LoadStore()
    {
        double baseSpeed = GameConstants.relativeSpeed * GameConstants.speedMultiplier;

        // Small Airplane 
        smallAirplanePrice.text = $"Price: {GameConstants.smallPrice} coins";
        smallAirplaneCapacity.text = $"Capacity: {GameConstants.smallCapacity}";
        smallAirplaneSpeed.text = $"Speed: {(int)(baseSpeed * GameConstants.smallSpeedMultiplier * 5)}  km/h";
        smallAirplaneRange.text = $"Range: {GameConstants.smallRange} km";
        smallAirplaneBuy.onClick.AddListener(smallAirplaneBought);

        // Medium Airplane 
        mediumAirplanePrice.text = $"Price: {GameConstants.mediumPrice} coins";
        mediumAirplaneCapacity.text = $"Capacity: {GameConstants.mediumCapacity}";
        mediumAirplaneSpeed.text = $"Speed: {(int)(baseSpeed * GameConstants.mediumSpeedMultiplier * 5)} km/h";
        mediumAirplaneRange.text = $"Range: {GameConstants.mediumRange} km";
        mediumAirplaneBuy.onClick.AddListener(mediumAirplaneBought);

        // Large Airplane 
        largeAirplanePrice.text = $"Price: {GameConstants.largePrice} coins";
        largeAirplaneCapacity.text = $"Capacity: {GameConstants.largeCapacity}";
        largeAirplaneSpeed.text = $"Speed: {(int)(baseSpeed * GameConstants.largeSpeedMultiplier * 5)} km/h";
        largeAirplaneRange.text = $"Range: {GameConstants.largeRange} km";
        largeAirplaneBuy.onClick.AddListener(largeAirplaneBought);

        // Dropdowns
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (string airportName in _info.savedAirports.Keys)
        {
            options.Add(new TMP_Dropdown.OptionData(airportName));
        }

        smallAirplaneSelector.ClearOptions();
        mediumAirplaneSelector.ClearOptions();
        largeAirplaneSelector.ClearOptions();

        smallAirplaneSelector.AddOptions(options);
        mediumAirplaneSelector.AddOptions(options);
        largeAirplaneSelector.AddOptions(options);
    }

    public void Awake()
    {
        UIEvents.LoadedListeners.AddListener(LoadStore);
        gameObject.SetActive(false);
    }
}