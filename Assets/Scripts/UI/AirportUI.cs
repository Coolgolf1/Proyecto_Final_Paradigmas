using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AirportUI : MonoBehaviour
{
    [SerializeField] private TMP_Text airportID;

    [SerializeField] private TMP_Text airportName;
    [SerializeField] private TMP_Text passengers;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text numAirplanes;
    [SerializeField] private Button buyAirplanes;
    [SerializeField] private Button upgradeAirport;
    [SerializeField] private AirplaneStore store;
    [SerializeField] private TMP_Text maxClients;
    [SerializeField] private Button activatePriority;

    private InfoSingleton _info = InfoSingleton.GetInstance();

    private Airport activeAirport;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
        closeButton.onClick.AddListener(CloseUI);
        buyAirplanes.onClick.AddListener(BuyAirplanes);
        activatePriority.onClick.AddListener(TogglePriority);

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
    }

    public void ShowAirport(Airport airport)
    {
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
        string passengersText = "";
        foreach (Airport destAirport in Player.UnlockedAirports)
        {
            if (destAirport != activeAirport)
                passengersText += $"- {destAirport.Id.ToUpper()}: {activeAirport.TravellersToAirport[destAirport]}\n";

        }

        //passengersText += $"\n- En Destino Final: {airport.ReceivedTravellers} pasajeros\n";

        passengers.text = passengersText;

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
}