using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlightUI : MonoBehaviour
{
    [SerializeField] private TMP_Text flightNumber;
    [SerializeField] private TMP_Text routeText;
    [SerializeField] private TMP_Text passengers;
    [SerializeField] private Button closeButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        flightNumber = GameObject.Find("FlightNumber").GetComponent<TMP_Text>();
        routeText = GameObject.Find("RouteText").GetComponent<TMP_Text>();
        passengers = GameObject.Find("FlightClientList").GetComponent<TMP_Text>();
        closeButton = GameObject.Find("CloseFlightUI").GetComponent<Button>();
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
    }

    public void ShowFlight(Flight flight)
    {
        flightNumber.text = flight.FlightID;
        routeText.text = $"{flight.AirportOrig.name}-{flight.AirportDest.name}";
        string passengersText = "";
        foreach (Airport destAirport in flight.TravellersToAirport.Keys)
        {
            passengersText += $"- {destAirport.name}: {flight.TravellersToAirport[destAirport]}\n";
        }

        passengers.text = passengersText;
    }
}