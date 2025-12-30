using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject sun;
    [SerializeField] GameObject audioManager;

    [Header("Graphics")]
    [SerializeField] TMP_Dropdown qualitySelector;

    [Header("Sound")]
    [SerializeField] Slider masterVolume;
    [SerializeField] Slider musicVolume;
    [SerializeField] Toggle sfxToggle;

    [Header("Reset Buttons")]
    [SerializeField] Button resetHighScore;
    [SerializeField] Button resetTutorial;
    [SerializeField] Button confirmReset;
    [SerializeField] Button cancelReset;

    [Header("Menu")]
    [SerializeField] Button backToMenu;
    [SerializeField] GameObject background;
    [SerializeField] GameObject toggleImage;
    [SerializeField] GameObject resetMessage;
    [SerializeField] TMP_Text currentHighScore;

    private bool _checkActive = false;

    private string _qualityId = "quality";
    private string _masterVolumeId = "masterVolume";
    private string _musicVolumeId = "musicVolume";
    private string _sfxToggleId = "sfxToggle";

    SoundController musicController;
    private void SavePlayerPrefs()
    {
        PlayerPrefs.SetInt(_qualityId, qualitySelector.value);
        PlayerPrefs.SetFloat(_masterVolumeId, masterVolume.value);
        PlayerPrefs.SetFloat(_musicVolumeId, musicVolume.value);
        PlayerPrefs.SetInt(_sfxToggleId, sfxToggle.isOn ? 1 : 0);

        PlayerPrefs.Save();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.HasKey(_qualityId))
            qualitySelector.value = PlayerPrefs.GetInt(_qualityId);
        else
            qualitySelector.value = 2;

        if (PlayerPrefs.HasKey(_masterVolumeId))
            masterVolume.value = PlayerPrefs.GetFloat(_masterVolumeId);
        else masterVolume.value = 1;

        if (PlayerPrefs.HasKey(_musicVolumeId))
            musicVolume.value = PlayerPrefs.GetFloat(_musicVolumeId);
        else
            musicVolume.value = 1;

        if (PlayerPrefs.HasKey(_sfxToggleId))
            sfxToggle.isOn = PlayerPrefs.GetInt(_sfxToggleId) == 1;
        else sfxToggle.isOn = true;
        
        backToMenu.onClick.AddListener(ExitSettings);
        UIEvents.OnSettingsEnter.AddListener(EnterSettings);

        musicController = audioManager.gameObject.GetComponent<SoundController>();

        musicVolume.onValueChanged.AddListener(HandleMusic);
        masterVolume.onValueChanged.AddListener(HandleMaster);
        sfxToggle.onValueChanged.AddListener(HandleToggle);

        qualitySelector.onValueChanged.AddListener(HandleQuality);

        resetHighScore.onClick.AddListener(HandleScoreReset);

        HandleMaster(masterVolume.value); 
        HandleMusic(musicVolume.value); 
        HandleQuality(qualitySelector.value); 
        HandleToggle(sfxToggle.isOn);

        currentHighScore.text = $"Current Highscore: {Player.GetHighScore()}";

        resetMessage.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void HandleScoreReset()
    {
        currentHighScore.text = $"Current Highscore: {Player.GetHighScore()}";
        resetMessage.gameObject.SetActive(true);
        confirmReset.onClick.AddListener(ResetScore);
        cancelReset.onClick.AddListener(CancelResetScore);
    }

    private void ResetScore()
    {
        Player.ResetHighScore();
        confirmReset.onClick.RemoveAllListeners();
        cancelReset.onClick.RemoveAllListeners();
        resetMessage.gameObject.SetActive(false);
    }

    private void CancelResetScore()
    {
        confirmReset.onClick.RemoveAllListeners();
        cancelReset.onClick.RemoveAllListeners();
        resetMessage.gameObject.SetActive(false);
    }

    private void HandleMaster(float value)
    {
        musicController.SetMasterVolume(value);
    }

    private void HandleMusic(float value)
    {
        musicController.SetMusicVolume(value);
    }

    public void HandleQuality(int quality)
    {
        QualitySettings.SetQualityLevel(quality);
    }

    public void HandleToggle(bool isOn)
    {
        musicController.ToggleSFX(isOn);
        toggleImage.gameObject.SetActive(isOn);
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
        SavePlayerPrefs();
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
