using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEndUI : MonoBehaviour
{
    [Header("Text Fields")]
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text gameScoreText;
    [SerializeField] private TMP_Text newRecordText;


    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private GameMaster _gm = GameMaster.GetInstance();

    public void Start()
    {
        UIEvents.OnEndGameEnter.AddListener(ShowEndGameUI);
        UIEvents.OnEndGameExit.AddListener(ExitEndGameUI);
        gameObject.SetActive(false);

        // Button listeners
        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(MainMenu);
    }

    public void ShowEndGameUI()
    {
        int currentScore = Player.Score;
        int highScore = Player.GetHighScore();

        if (currentScore > highScore)
        {
            Player.SaveHighScore();
            newRecordText.gameObject.SetActive(true);
            gameScoreText.gameObject.SetActive(false);

        } else
        {
            newRecordText.gameObject.SetActive(false);
            gameScoreText.gameObject.SetActive(true);
            gameScoreText.text = $"Game Score: {currentScore}";
        }

        highScoreText.text = $"High Score: {Player.GetHighScore()}";
        
        gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        UIEvents.OnRestartGame?.Invoke();
        _gm.ChangeState(_gm.Play);
    }

    public void MainMenu()
    {
        _gm.ChangeState(_gm.MainMenu);
    }

    public void ExitEndGameUI()
    {
        gameObject.SetActive(false);
    }
}
