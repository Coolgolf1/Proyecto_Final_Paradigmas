using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AirportUI : MonoBehaviour
{
    [SerializeField] private TMP_Text airportID;

    [SerializeField] private TMP_Text airportName;
    [SerializeField] private TMP_Text passengers;
    [SerializeField] private Button closeButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        airportID = GameObject.Find("AirportID").GetComponent<TMP_Text>();
        airportName = GameObject.Find("CityName").GetComponent<TMP_Text>();
        passengers = GameObject.Find("AirportClientList").GetComponent<TMP_Text>();
        closeButton = GameObject.Find("CloseAirportUI").GetComponent<Button>();
        closeButton.onClick.AddListener(CloseUI);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CloseUI()
    {
        gameObject.SetActive(false);
    }


    public void ShowAirport(Airport airport)
    {
        airportID.text = airport.Id.ToUpper();
        airportName.text = airport.name;
        string passengersText = "";
        foreach (Airport destAirport in airport.TravellersToAirport.Keys)
        {
            passengersText += $"- {destAirport.name}: {airport.TravellersToAirport[destAirport]}\n";
        }

        passengersText += $"\n- En Destino Final: {airport.ReceivedTravellers} pasajeros\n";

        passengers.text = passengersText;
    }
}
