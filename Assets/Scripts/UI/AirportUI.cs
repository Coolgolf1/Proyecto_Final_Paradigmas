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

    private InfoSingleton _info = InfoSingleton.GetInstance();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
        closeButton.onClick.AddListener(CloseUI);
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
    }

    public void ShowAirport(Airport airport)
    {
        if (airportID == null) Debug.LogError("¡airportID (el componente de texto) es NULL!");
        if (airport == null) Debug.LogError("¡El objeto airport que recibo es NULL!");
        else if (airport.Id == null) Debug.LogError("¡El string Id del aeropuerto es NULL!");

        if (_info.savedRoutes != null)
        {
            foreach (Route route in _info.savedRoutes.Values)
            {
                if (route != null && route.lit) route.UnlitRoute();
            }
        }


        airportID.text = airport.Id.ToUpper();
        airportName.text = airport.name;
        string passengersText = "";
        foreach (Airport destAirport in airport.TravellersToAirport.Keys)
        {
            passengersText += $"- {destAirport.name}: {airport.TravellersToAirport[destAirport]}\n";
        }

        passengersText += $"\n- En Destino Final: {airport.ReceivedTravellers} pasajeros\n";

        passengers.text = passengersText;

        numAirplanes.text = $"Number of airplanes: {airport.Hangar.Count}";
    }
}