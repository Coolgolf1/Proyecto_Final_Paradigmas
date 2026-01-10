using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    private float _elapsedTime;

    [Header("Sprites")]
    [SerializeField] private Sprite muteSprite;
    [SerializeField] private Sprite volumeSprite;

    [Header("Buttons")]
    [SerializeField] private Button play;
    [SerializeField] private Button pause;
    [SerializeField] private Button fastForward;
    [SerializeField] private Button muteButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitPromptButton;
    [SerializeField] private Button homeButton;

    [Header("Text Fields")]
    [SerializeField] private TMP_Text timeCounter;
    [SerializeField] private TMP_Text dayCounter;
    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text money;

    [Header("Others")]
    [SerializeField] private GameObject exitCanvas;

    private int _days = 0;

    private bool _enabled = false;

    private long _currentMoney = 0;

    private bool muteStatus = false;

    private EconomyManager _economy = EconomyManager.GetInstance();
    private InfoSingleton _info = InfoSingleton.GetInstance();
    private GameMaster _gm = GameMaster.GetInstance();

    private Tuple<int, int, int> refTime = Tuple.Create(9, 0, 0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _elapsedTime = 0;
        play.onClick.AddListener(Play);
        pause.onClick.AddListener(Pause);
        fastForward.onClick.AddListener(FastForward);
        _economy.MoneyChange += HandleMoneyChange;
        muteButton.onClick.AddListener(ToggleMute);
        homeButton.onClick.AddListener(ExitToMenu);
        settingsButton.onClick.AddListener(OpenSettings);
        exitPromptButton.onClick.AddListener(ToggleExitPrompt);


        UIEvents.OnPlayEnter.AddListener(StartGame);
        UIEvents.OnMainMenuEnter.AddListener(HideUI);
        UIEvents.OnSettingsExit.AddListener(ShowUI);

        // Finished loading listeners
        UIEvents.LoadedListeners?.Invoke();

        UIEvents.OnEndGameEnter.AddListener(HideUI);
    }

    private void OpenSettings()
    {
        exitCanvas.SetActive(false);
        if (_info.playerCamera.GetComponent<PlayerMovement>() is SpaceCamera camera)
            camera.ComingFromGame = true;
        UIEvents.OnSettingsEnter?.Invoke();
        Time.timeScale = 0;
        HideUI();
    }

    private void Play()
    {
        Time.timeScale = 1;
    }

    private void Pause()
    {
        Time.timeScale = 0;
    }

    private void FastForward()
    {
        Time.timeScale = 4;
    }

    private void HandleMoneyChange(object sender, EventArgs e)
    {
        _currentMoney = Player.Money;
    }

    private void ToggleExitPrompt()
    {
        if (exitCanvas.activeSelf)
            exitCanvas.SetActive(false);
        else
            exitCanvas.SetActive(true);
    }

    private void ExitToMenu()
    {
        exitCanvas.SetActive(false);
        _gm.ChangeState(_gm.End);
        _gm.ChangeState(_gm.MainMenu);
       
    }

    private void ToggleMute()
    {
        Image icon = muteButton.GetComponent<Image>();

        if (muteStatus)
        {
            icon.sprite = volumeSprite;
        }
        else
        {
            icon.sprite = muteSprite;
        }

        _info.ToggleMute();
        muteStatus = !muteStatus;
    }


    // Update is called once per frame
    private void Update()
    {
        if (_enabled)
        {
            _elapsedTime += Time.deltaTime * 1000;

            _days = (int)Mathf.Floor((_elapsedTime + refTime.Item1 * 3600) / 86400);
            string hours = ((Mathf.Floor(_elapsedTime / 3600) + refTime.Item1) % 24).ToString("00");
            string minutes = ((Mathf.Floor(_elapsedTime / 60) + refTime.Item2) % 60).ToString("00");
            
            timeCounter.text = $"{hours}:{minutes}";
            dayCounter.text = $"{_days}";

            score.text = $"{Auxiliary.FormatValue(Player.Score)}";
            money.text = $"{Auxiliary.FormatValue(_currentMoney)}";
        }
    }

    public void StartGame()
    {
        gameObject.SetActive(true);

        // Call deltaTime to restart delta
        _ = Time.deltaTime;
        _elapsedTime = 0;

        _enabled = true;
        gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;
        exitCanvas.SetActive(false);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        if (_info.playerCamera.GetComponent<PlayerMovement>() is SpaceCamera camera) 
            if (camera.ComingFromGame)
                gameObject.SetActive(true);
    }
}