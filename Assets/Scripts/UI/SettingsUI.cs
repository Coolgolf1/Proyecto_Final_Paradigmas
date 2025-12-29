using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject sun;

    [Header("Graphics")]
    [SerializeField] TMP_Dropdown qualitySelector;

    [Header("Sound")]
    [SerializeField] Slider masterVolume;
    [SerializeField] Slider musicVolume;

    [Header("Reset Buttons")]
    [SerializeField] Button resetHighScore;
    [SerializeField] Button resetTutorial;

    [Header("Menu")]
    [SerializeField] Button backToMenu;
    [SerializeField] GameObject background;

    private Vector3 _camPosition = new Vector3(3.74000001f, -31.1100006f, 6.69000006f);
    private Quaternion _camRotation = new Quaternion(-0.2301296f, -0.702412069f, -0.428652734f, 0.519532919f);
    private Quaternion _sunRotation = new Quaternion(-0.288833886f, 0.484140009f, 0.719054401f, -0.406379402f);

    private bool _startAnimation = false;
    private bool _startMenuAnimation = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        masterVolume.value = 0.8f;
        musicVolume.value = 0.6f;
        backToMenu.onClick.AddListener(ExitSettings);
        UIEvents.OnSettingsEnter.AddListener(EnterSettings);
        gameObject.SetActive(false);    
    }

    public void HideUI()
    {
        background.SetActive(false);
    }

    public void ShowUI()
    {
        background.SetActive(true);
    }

    public void EnterSettings()
    {
        gameObject.SetActive(true);
        _startAnimation = true;
    }

    public void ExitSettings()
    {
        UIEvents.OnSettingsExit?.Invoke();
        _startMenuAnimation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_startAnimation)
        {
            SpaceCamera camera = player.GetComponent<SpaceCamera>();
            camera.GoingToMenu = false;
            player.transform.position = Vector3.Lerp(player.transform.position, _camPosition, 3*Time.deltaTime);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, _camRotation, 3*Time.deltaTime);
            sun.transform.rotation = Quaternion.Lerp(sun.transform.rotation, _sunRotation, Time.deltaTime);

            // If not close enough to initial view
            if ((_camPosition - player.transform.position).magnitude >= 0.1)
                return;

            // Animation finished
            player.transform.position = _camPosition;
            player.transform.rotation = _camRotation;
            sun.transform.rotation = _sunRotation;
            _startAnimation = false;
            UIEvents.OnSettingsLoaded?.Invoke();
        }

        if (_startMenuAnimation)
        {
            SpaceCamera camera = player.GetComponent<SpaceCamera>();
            camera.GoingToMenu = true;
            camera.GoToMainMenu();
            sun.transform.rotation = Quaternion.Lerp(GameConstants.menuSunRotation, sun.transform.rotation, Time.deltaTime);
            if (!camera.GoingToMenu)
            {
                sun.transform.rotation = GameConstants.menuSunRotation;
                gameObject.SetActive(false);
                _startMenuAnimation = false;
            }
        }
    }
}
