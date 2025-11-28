using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    private float _elapsedTime;
    [SerializeField] private TMP_Text timeCounter;
    [SerializeField] private TMP_Text dayCounter;
    [SerializeField] private Button play;
    [SerializeField] private Button pause;
    [SerializeField] private Button fastForward;
    [SerializeField] private TMP_Text money;

    private int _days = 0;

    private bool _enabled = false;

    private int _currentMoney = 0;

    private EconomyManager _economy = EconomyManager.GetInstance();

    private Tuple<int, int, int> refTime = Tuple.Create(9, 0, 0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _elapsedTime = 0;
        play.onClick.AddListener(Play);
        pause.onClick.AddListener(Pause);
        fastForward.onClick.AddListener(FastForward);
        _economy.MoneyChange += HandleMoneyChange;

        UIEvents.OnPlayEnter.AddListener(StartGame);
        UIEvents.OnMainMenuEnter.AddListener(HideUI);

        // Finished loading listeners
        UIEvents.LoadedListeners?.Invoke();
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

    // Update is called once per frame
    private void Update()
    {
        if (_enabled)
        {
            _elapsedTime += Time.deltaTime * 1000;

            _days = (int)Mathf.Floor((_elapsedTime + refTime.Item1*3600) / 86400);
            string hours = ((Mathf.Floor(_elapsedTime / 3600) + refTime.Item1) % 24).ToString("00");
            string minutes = ((Mathf.Floor(_elapsedTime / 60) + refTime.Item2) % 60).ToString("00");
            //string seconds = ((Mathf.Floor(_elapsedTime % 60) + refTime.Item3) % 60).ToString("00");
            timeCounter.text = $"{hours}:{minutes}";
            money.text = $"Coins: {_currentMoney}";
            dayCounter.text = $"Day: {_days}";
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
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}