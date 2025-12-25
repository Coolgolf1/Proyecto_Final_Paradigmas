using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlightUI : MonoBehaviour
{
    [SerializeField] private TMP_Text flightNumber;
    [SerializeField] private TMP_Text routeText;
    [SerializeField] private TMP_Text passengers;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button upgradePlane;
    [SerializeField] private Button viewPlane;
    private Airplane _linkedAirplane;
    private InfoSingleton _info = InfoSingleton.GetInstance();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
        closeButton.onClick.AddListener(CloseUI);
        upgradePlane.onClick.AddListener(OnUpgrade);
        viewPlane.onClick.AddListener(OnView);
        gameObject.SetActive(false);

        UIEvents.OnEndGameEnter.AddListener(CloseUI);

        UIEvents.OnAirplaneStoreEnter.AddListener(CloseUI);
        UIEvents.OnRouteStoreEnter.AddListener(CloseUI);

        UIEvents.OnMainMenuEnter.AddListener(CloseUI);
        
    }

    // Update is called once per frame
    private void Update()
    {
    }

    

    private void OnUpgrade()
    {
        if (_linkedAirplane != null)
        {
            CloseUI();
            _info.GoToHangar(_linkedAirplane);
        }
    }

    private void OnView()
    {
        if (_info.playerCamera.GetComponent<PlayerMovement>() is SpaceCamera camera)
        {
            camera.SetAirplane(_linkedAirplane);
            CloseUI();
        }
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

    public void ShowFlight(Flight flight)
    {
        _linkedAirplane = flight.Airplane;
        foreach (Route route in _info.savedRoutes.Values)
        {
            if (route.lit)
            {
                route.UnlitRoute();
            }
        }

        flightNumber.text = flight.FlightID;
        routeText.text = $"{flight.AirportOrig.Id.ToUpper()} - {flight.AirportDest.Id.ToUpper()}";
        string passengersText = "";
        foreach (Airport destAirport in Player.UnlockedAirports)
        {
            passengersText += $"- {destAirport.Id.ToUpper()}: {flight.TravellersToAirport[destAirport]}\n";
        }

        passengers.text = passengersText;
    }
}