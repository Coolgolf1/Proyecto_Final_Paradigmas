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

    private bool _checkActive = false;

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
        SpaceCamera camera = player.GetComponent<SpaceCamera>();
        _checkActive = false;
        camera.Sun = sun;
        camera.GoingToMenu = false;
        camera.GoingBackToGame = false;
        camera.GoingToSettings = true;
        
        UIEvents.OnSettingsLoaded?.Invoke();
    }

    public void ExitSettings()
    {
        UIEvents.OnSettingsExit?.Invoke();
        SpaceCamera camera = player.GetComponent<SpaceCamera>();
        camera.UseSlerp = true;
        camera.Sun = sun;
        camera.GoingToSettings = false;
        if (camera.ComingFromGame)
            camera.GoingBackToGame = true;
        else
            camera.GoingToMenu = true;
        
        _checkActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_checkActive)
           CheckActive();
        
    }

    void CheckActive()
    {
        SpaceCamera camera = player.GetComponent<SpaceCamera>();
        if (gameObject.activeSelf && !camera.GoingToMenu && !camera.GoingBackToGame) { gameObject.SetActive(false); _checkActive = false; }
    }
}
