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

    private InfoSingleton _info = InfoSingleton.GetInstance();

    private Airport activeAirport;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
        closeButton.onClick.AddListener(CloseUI);
        buyAirplanes.onClick.AddListener(BuyAirplanes);

        UIEvents.OnAirplaneStoreEnter.AddListener(CloseUI);
        UIEvents.OnRouteStoreEnter.AddListener(CloseUI);
        UIEvents.OnEndGameEnter.AddListener(CloseUI);
        UIEvents.OnMainMenuEnter.AddListener(CloseUI);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
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

        airportID.text = airport.Id.ToUpper();
        airportName.text = airport.Name;
        maxClients.text = $"{airport.Capacity} max";
        string passengersText = "";
        foreach (Airport destAirport in Player.UnlockedAirports)
        {
            if (destAirport != airport)
                passengersText += $"- {destAirport.Id.ToUpper()}: {airport.TravellersToAirport[destAirport]}\n";
            
        }

        //passengersText += $"\n- En Destino Final: {airport.ReceivedTravellers} pasajeros\n";

        passengers.text = passengersText;

        numAirplanes.text = $"{airport.Hangar.Count}";
    }

    private void BuyAirplanes()
    {
        store.OpenStoreFor(activeAirport);
        
    }
}