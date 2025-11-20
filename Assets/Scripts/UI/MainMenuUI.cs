using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private Button startGame;
    [SerializeField] private Button options;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject player;

    private bool _startAnimation;

    private void OnEnable()
    {
        UIEvents.OnMainMenuEnter += ShowButtons;
        UIEvents.OnMainMenuExit += HideButtons;
    }

    private void OnDisable()
    {
        UIEvents.OnMainMenuEnter -= ShowButtons;
        UIEvents.OnMainMenuExit -= HideButtons;
    }

    public void HideButtons()
    {
        title.gameObject.SetActive(false);

        startGame.interactable = false;
        startGame.gameObject.GetComponent<Image>().enabled = false;
        startGame.gameObject.GetComponentInChildren<TMP_Text>().enabled = false;

        options.interactable = false;
        options.gameObject.GetComponent<Image>().enabled = false;
        options.gameObject.GetComponentInChildren<TMP_Text>().enabled = false;

        exitButton.interactable = false;
        exitButton.gameObject.GetComponent<Image>().enabled = false;
        exitButton.gameObject.GetComponentInChildren<TMP_Text>().enabled = false;
    }

    public void ShowButtons()
    {
        title.gameObject.SetActive(true);

        startGame.interactable = true;
        startGame.gameObject.GetComponent<Image>().enabled = true;
        startGame.gameObject.GetComponentInChildren<TMP_Text>().enabled = true;

        options.interactable = true;
        options.gameObject.GetComponent<Image>().enabled = true;
        options.gameObject.GetComponentInChildren<TMP_Text>().enabled = true;

        exitButton.interactable = true;
        exitButton.gameObject.GetComponent<Image>().enabled = true;
        exitButton.gameObject.GetComponentInChildren<TMP_Text>().enabled = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startGame.onClick.AddListener(StartLoad);
        //earth.GetComponent<SpinFree>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (_startAnimation)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, GameConstants.initCameraPosition, 3 * Time.deltaTime);

            // If not close enough to initial view
            if ((GameConstants.initCameraPosition - player.transform.position).magnitude >= 0.1)
                return;

            // Animation finished
            player.transform.position = GameConstants.initCameraPosition;
            _startAnimation = false;
            gameObject.SetActive(false);
        }
    }

    void StartLoad()
    {
        _startAnimation = true;

        // Change state to play
        FindFirstObjectByType<GameMaster>().ChangeState(FindFirstObjectByType<GameMaster>().Play);
    }
}
