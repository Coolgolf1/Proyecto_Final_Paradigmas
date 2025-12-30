using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class SpaceCamera : PlayerMovement
{
    private Vector2 zoomValue;
    private float modulo;
    [SerializeField] private float moduloPower = 1.8f;
    [SerializeField] private float zoomDecay = 8f;
    [SerializeField] private float zoomSensitivity = 2500f;

    [SerializeField] private AudioClip defaultMusic;
    [SerializeField] private AudioClip alertMusic;


    private float targetZoom = 0;
    private float previousZoom = 0;
    private float actualZoom = 0;

    private Airplane followingAirplane;
    private bool _arrivedAirplane;
    private bool _arrivedAirport;

    private Vector3 gamePosition;
    private Quaternion gameRotation;
    private Quaternion? sunGameRotation;

    public bool GoingToMenu { get; set; }
    public bool GoingToInit { get; set; }
    private bool _goingToSettings;
    public bool GoingToSettings {
        get { return _goingToSettings; }
        set { _goingToSettings = value;
            if (value)
            {
                gamePosition = transform.position;
                gameRotation = transform.rotation;
                sunGameRotation = Sun is null ? null : Sun.transform.rotation;
            } }
    }
    public bool GoingBackToGame { get; set; }
    public bool ComingFromGame { get; set; }

    private bool _alert = false;
    private int activatedCount = 0;

    public bool UseSlerp { get; set; }
    public GameObject Sun { get; set; }

    private Airport airportObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        UIEvents.OnEndGameEnter.AddListener(StopFollowing);
        UIEvents.OnEndGameEnter.AddListener(StopMusic);
        UIEvents.OnRestartGame.AddListener(RestartSound);
        UIEvents.OnMainMenuEnter.AddListener(RestartSound);
        GoingToMenu = false;
        GoingToInit = false;

        GetComponent<AudioSource>().clip = defaultMusic;
        GetComponent<AudioSource>().Play();
    }

    public void RestartSound()
    {
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().clip = defaultMusic;
        GetComponent<AudioSource>().Play();
    }

    public void ActivateAlertMusic()
    {
        activatedCount++;
        if (!_alert)
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().clip = alertMusic;
            GetComponent<AudioSource>().Play();
            _alert = true;
        }
    }

    public void DeactivateAlertMusic(bool mainMenu = false)
    {
        activatedCount--;
        if (_alert && (activatedCount <= 0 || mainMenu))
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().clip = defaultMusic;
            GetComponent<AudioSource>().Play();
            _alert = false;
            activatedCount = 0;
        }
    }

    public void StopMusic()
    {
        GetComponent<AudioSource>().Stop();
    }

    public void StopFollowing()
    {
        followingAirplane = null;
        airportObj = null;
        
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (_enabled)
        {
            SmoothZoom();
        }

        if (followingAirplane is not null)
        {
            FollowAirplane();
        }
        else
        {
            _arrivedAirplane = false;
        }

        if (airportObj is not null)
        {
            GoToAirport();
        }
        else
        {
            _arrivedAirport = false;
        }

        if (GoingToMenu)
        {
            GoToMainMenu();
        } else if (GoingToInit)
        {
            GoToInit();
        } else if (GoingToSettings)
        {
            drag.Disable();
            zoom.Disable();
            GoToSettings();
        } else if (GoingBackToGame)
        {
            GoBackToGame();
        }

    }

    private void SmoothZoom()
    {
        zoomValue = zoom.ReadValue<Vector2>();
        modulo = transform.position.magnitude;

        zoomFactor = (Mathf.Pow(modulo, moduloPower) / zoomSensitivity);

        if ((modulo > 30 || zoomValue[1] < 0) && (modulo < 75 || zoomValue[1] > 0))
        {
            targetZoom += zoomValue[1];
        }

        actualZoom = Mathf.Lerp(actualZoom, targetZoom, Time.fixedDeltaTime * zoomDecay);

        transform.position += transform.forward * (actualZoom - previousZoom);

        previousZoom = actualZoom;
    }

    public void SetAirplane(Airplane airplane)
    {
        followingAirplane = airplane;
        airportObj = null;
    }

    public void SetAirport(Airport airport)
    {
        followingAirplane = null;
        airportObj = airport;
    }

    

    private void FollowAirplane()
    {
        if (drag.IsPressed())
        {
            StopFollowing();
            return;
        }


        Vector3 earthCenter = _info.earth.transform.position;

        Vector3 airplanePos = followingAirplane.transform.position;

        Vector3 direction = (airplanePos - earthCenter).normalized;
        float distanceOver = 15f;

        Vector3 objPosition = airplanePos + direction * distanceOver;

        if (!_arrivedAirplane)
        {
            transform.position = Vector3.Slerp(transform.position, objPosition, Time.fixedDeltaTime);
            //Quaternion finalRotation = Quaternion.LookRotation(direction, Vector3.up);
            //transform.rotation = Quaternion.Lerp(transform.rotation, finalRotation, 3 * Time.deltaTime);

            // If not close enough to initial view
            if ((objPosition - transform.position).magnitude < 0.5)
                _arrivedAirplane = true;

        }

        else
        {
            transform.position = objPosition;

        }
        transform.LookAt(_info.earth.transform.position);
    }

    private void GoToAirport()
    {

        if (drag.IsPressed())
        {
            _arrivedAirport = true;
            airportObj = null;
        }

        if (!_arrivedAirport) {
            Vector3 earthCenter = _info.earth.transform.position;

            Vector3 airportPos = airportObj.transform.position;

            Vector3 direction = (airportPos - earthCenter).normalized;
            float distanceOver = 20f;

            Vector3 objPosition = airportPos + direction * distanceOver;

            transform.position = Vector3.Slerp(transform.position, objPosition, Time.fixedDeltaTime);
            
            
            // If not close enough to initial view
            if ((objPosition - transform.position).magnitude < 0.5)
            {
                _arrivedAirport = true;
                transform.position = objPosition;
                airportObj = null;
            }
            transform.LookAt(_info.earth.transform.position);
        } 

    }

    public void GoToMainMenu()
    {
        GoingBackToGame = false;
        GoingToSettings = false;
        if (UseSlerp)
        {
            transform.position = Vector3.Slerp(transform.position, GameConstants.mainMenuCameraPosition, Time.fixedDeltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, GameConstants.mainMenuCameraPosition, Time.fixedDeltaTime);
        }

        if (Sun is not null)
        {
            Sun.transform.rotation = Quaternion.Lerp(GameConstants.menuSunRotation, Sun.transform.rotation, Time.fixedDeltaTime);
        }
           
        transform.rotation = Quaternion.Lerp(transform.rotation, GameConstants.mainMenuCameraRotation, Time.fixedDeltaTime);

        // If not close enough to initial view
        if ((GameConstants.mainMenuCameraPosition - transform.position).magnitude >= 0.1)
            return;

        // Animation finished
        transform.position = GameConstants.mainMenuCameraPosition;
        transform.rotation = GameConstants.mainMenuCameraRotation;
        if (Sun is not null)
            Sun.transform.rotation = GameConstants.menuSunRotation;
        GoingToMenu = false;
        UseSlerp = false;
        
        
    }

    public void GoToSettings()
    {
        GoingToMenu = false;
        GoingBackToGame = false;
        transform.position = Vector3.Slerp(transform.position, GameConstants.settingsCameraPosition, Time.fixedDeltaTime / 2.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, GameConstants.settingsCameraRotation, Time.fixedDeltaTime / 2.0f);

        if (Sun is not null)
        {
            Sun.transform.rotation = Quaternion.Lerp(Sun.transform.rotation, GameConstants.settingsSunRotation, Time.fixedDeltaTime / 2.0f);
        }

        // If not close enough to initial view
        if ((GameConstants.settingsCameraPosition - transform.position).magnitude >= 0.1)
            return;

        // Animation finished
        transform.position = GameConstants.settingsCameraPosition;
        transform.rotation = GameConstants.settingsCameraRotation;
        if (Sun is not null)
            Sun.transform.rotation = GameConstants.settingsSunRotation;

        GoingToSettings = false;
        Sun = null;
    }

    public void GoToInit()
    {
        GoingToSettings = false;
        GoingToMenu = false;
        transform.position = Vector3.Lerp(transform.position, GameConstants.initCameraPosition, Time.fixedDeltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, GameConstants.initCameraRotation, Time.fixedDeltaTime);

        // If not close enough to initial view
        if ((GameConstants.initCameraPosition - transform.position).magnitude >= 0.1)
            return;

        // Animation finished
        transform.position = GameConstants.initCameraPosition;
        transform.rotation = GameConstants.initCameraRotation;
        GoingToInit = false;
        
    }

    public void GoBackToGame()
    {
        
        GoingToMenu = false;
        GoingToSettings = false;

        transform.position = Vector3.Slerp(transform.position, gamePosition, Time.fixedDeltaTime / 2.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, gameRotation, Time.fixedDeltaTime / 2.0f);

        if (Sun is not null && sunGameRotation is not null)
        {
            Sun.transform.rotation = Quaternion.Lerp(Sun.transform.rotation, (Quaternion)sunGameRotation, Time.fixedDeltaTime / 2.0f);
        }

        // If not close enough to initial view
        if ((gamePosition - transform.position).magnitude >= 0.1)
            return;

        // Animation finished
        transform.position = gamePosition;
        transform.rotation = gameRotation;
        if (Sun is not null && sunGameRotation is not null)
            Sun.transform.rotation = (Quaternion)sunGameRotation;

        GoingBackToGame = false;
        
        ComingFromGame = false;
        drag.Enable();
        zoom.Enable();
        Time.timeScale = 1;
    }
}
