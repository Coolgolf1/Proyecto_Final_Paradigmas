using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEndUI : MonoBehaviour
{
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text gameScoreText;
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
        int currentScore = Score.GetScore();
        int highScore = Score.GetHighScore();

        if (currentScore > highScore)
        {
            Score.SaveHighScore();
        }

        highScoreText.text = $"High Score: {highScore}";
        gameScoreText.text = $"Game Score: {currentScore}";

        gameObject.SetActive(true);
    }

    public void RestartGame()
    {
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
