using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    private float _elapsedTime;
    [SerializeField] private TMP_Text timeCounter;
    [SerializeField] private Button play;
    [SerializeField] private Button pause;
    [SerializeField] private Button fastForward;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _elapsedTime = 0;
        play.onClick.AddListener(Play);
        pause.onClick.AddListener(Pause);
        fastForward.onClick.AddListener(FastForward);
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
        Time.timeScale = 2;
    }

    // Update is called once per frame
    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        string minutes = Mathf.Floor(_elapsedTime / 60).ToString("00");
        string seconds = Mathf.Floor(_elapsedTime % 60).ToString("00");
        timeCounter.text = $"{minutes}:{seconds}";
    }
}