using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AirportUI : MonoBehaviour
{
    [Header("Text Fields")]
    [SerializeField] private TMP_Text airportID;
    [SerializeField] private TMP_Text airportName;
    [SerializeField] private TMP_Text passengers;
    [SerializeField] private TMP_Text numAirplanes;
    [SerializeField] private TMP_Text maxClients;
    [SerializeField] private TMP_Text upgradePrice;
    

    [Header("Buttons")]
    [SerializeField] private Button upgradeAirport;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button activatePriority;
    [SerializeField] private Button buyAirplanes;
    [SerializeField] private Button payUpgrade;
    [SerializeField] private Button closeUpgrade;

    [Header("Table")]
    [SerializeField] private TableUIManager tableManager;
    

    [Header("Others")]
    [SerializeField] private AirplaneStore store;
    [SerializeField] private GameObject upgradeMessage;

    private InfoSingleton _info = InfoSingleton.GetInstance();
    private EconomyManager _economy = EconomyManager.GetInstance();

    private Airport activeAirport;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
        closeButton.onClick.AddListener(CloseUI);
        buyAirplanes.onClick.AddListener(BuyAirplanes);
        activatePriority.onClick.AddListener(TogglePriority);

        closeUpgrade.onClick.AddListener(CloseUpgrade);
        upgradeAirport.onClick.AddListener(ShowUpgrade);
        payUpgrade.onClick.AddListener(PayUpgrade);

        UIEvents.OnAirplaneStoreEnter.AddListener(CloseUI);
        UIEvents.OnRouteStoreEnter.AddListener(CloseUI);
        UIEvents.OnEndGameEnter.AddListener(CloseUI);
        UIEvents.OnMainMenuEnter.AddListener(CloseUI);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (activeAirport is not null)
            UpdateStats();
    }

    private void CloseUI()
    {
        gameObject.SetActive(false);
        foreach (Route route in _info.savedRoutes.Values)
        {
            if (route.lit)
            {
                route.UnlitRoute();
            }
        }

        activeAirport = null;
        CloseUpgrade();
    }

    public void ShowAirport(Airport airport)
    {
        upgradeMessage.SetActive(false);
        activeAirport = airport;
        if (_info.savedRoutes != null)
        {
            foreach (Route route in _info.savedRoutes.Values)
            {
                if (route != null && route.lit) route.UnlitRoute();
            }
        }
    }

    private void UpdateStats()
    {
        airportID.text = activeAirport.Id.ToUpper();
        airportName.text = activeAirport.Name;
        maxClients.text = $"{activeAirport.Capacity} max";
        //string passengersText = "";
        //foreach (Airport destAirport in Player.UnlockedAirports)
        //{
        //    if (destAirport != activeAirport)
        //        passengersText += $"- {destAirport.Id.ToUpper()}: {activeAirport.TravellersToAirport[destAirport]}\n";

        //}

        //passengersText += $"\n- En Destino Final: {airport.ReceivedTravellers} pasajeros\n";

        //passengers.text = passengersText;

        tableManager.UpdateTable(activeAirport);

        numAirplanes.text = $"{activeAirport.Hangar.Count}";

        if (activeAirport.PriorityOn)
        {
            activatePriority.GetComponent<Image>().color = Color.lightGreen;
            activatePriority.GetComponentInChildren<TMP_Text>().text = "Priority On";
        }
        else
        {
            activatePriority.GetComponent<Image>().color = Color.red;
            activatePriority.GetComponentInChildren<TMP_Text>().text = "Priority Off";
        }

        int _upgradePrice = ((int)activeAirport.Level + 1) * 1500000;
        if (activeAirport.Level == Levels.Elite) {
            payUpgrade.interactable = false;
            upgradePrice.text = $"Max Level Reached";
        }
        else
        {
            upgradePrice.text = $"{_upgradePrice.ToString("#,#")} coins";
            if (_economy.GetBalance() < _upgradePrice)
            {
                payUpgrade.interactable = false;
            }
            else
            {
                payUpgrade.interactable = true;
            }

        }
        
    }

    private void ShowUpgrade()
    {
        upgradeMessage.SetActive(true);
    }

    private void CloseUpgrade()
    {
        upgradeMessage.SetActive(false);
    }

    private void BuyAirplanes()
    {
        store.OpenStoreFor(activeAirport);
        
    }

    private void TogglePriority()
    {
        if (activeAirport != null)
        {
            if (activeAirport.PriorityOn)
            {
                activeAirport.PriorityOn = false;
                _info.notificationSystem.AddNotification($"Disabled Priority in {activeAirport.Name}", "airport", "blue");
            }
            else
            {
                _info.EnablePriority(activeAirport);
                _info.notificationSystem.AddNotification($"Enabled Priority in {activeAirport.Name}", "airport", "green");
            }
        }
    }

    private void PayUpgrade()
    {
        if (activeAirport is not null)
        {
            int _upgradePrice = ((int)activeAirport.Level + 1) * 1500000;
            if (_economy.SubtractCoins(_upgradePrice))
            {
                activeAirport.Upgrade();
                _info.notificationSystem.AddNotification($"Airport {activeAirport.Name} upgraded", "airport", "green");
            }
        }
    }
}