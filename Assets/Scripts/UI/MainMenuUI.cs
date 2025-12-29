using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Image logo;
    [SerializeField] private Button startGame;
    [SerializeField] private Button options;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject player;

    private bool _startAnimation;

    private GameMaster _gameMaster = GameMaster.GetInstance();

    private void OnEnable()
    {
        UIEvents.OnMainMenuEnter.AddListener(ShowButtons);
        UIEvents.OnTransitionExit.AddListener(HideButtons);
    }

    private void OnDisable()
    {
        UIEvents.OnMainMenuEnter.RemoveListener(ShowButtons);
        UIEvents.OnTransitionExit.RemoveListener(HideButtons);
    }

    public void HideButtons()
    {
        logo.gameObject.SetActive(false);

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
        SpaceCamera camera = player.GetComponent<SpaceCamera>();
        camera.DeactivateAlertMusic(mainMenu: true);
        logo.gameObject.SetActive(true);

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
        exitButton.onClick.AddListener(Application.Quit);
        //earth.GetComponent<SpinFree>().enabled = true;
        UIEvents.OnMainMenuEnter.AddListener(RestartMenu);
        options.onClick.AddListener(LoadSettings);
        UIEvents.OnSettingsLoaded.AddListener(SettingsLoaded);
        UIEvents.OnSettingsExit.AddListener(OnSettingsExit);
    }

    public void RestartMenu()
    {
        SpaceCamera camera = player.GetComponent<SpaceCamera>();
        camera.GoingToMenu = true;
        gameObject.SetActive(true);
        ShowButtons();
    }

    // Update is called once per frame
    void Update()
    {

        if (_startAnimation)
        {
            SpaceCamera camera = player.GetComponent<SpaceCamera>();
            camera.GoingToMenu = false;
            player.transform.position = Vector3.Lerp(player.transform.position, GameConstants.initCameraPosition, 3 * Time.deltaTime);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, GameConstants.initCameraRotation, 3 * Time.deltaTime);

            // If not close enough to initial view
            if ((GameConstants.initCameraPosition - player.transform.position).magnitude >= 0.1)
                return;

            // Animation finished
            player.transform.position = GameConstants.initCameraPosition;
            player.transform.rotation = GameConstants.initCameraRotation;
            _startAnimation = false;
            HideButtons();
            gameObject.SetActive(false);

            // Change state to play
            StartPlay();
        }
    }

    public void StartLoad()
    {
        _startAnimation = true;
        _gameMaster.ChangeState(_gameMaster.Transition);
    }

    public void StartPlay()
    {
        _gameMaster.ChangeState(_gameMaster.Play);
    }

    public void LoadSettings()
    {
        UIEvents.OnSettingsEnter?.Invoke();
    }

    public void SettingsLoaded()
    {
        gameObject.SetActive(false);
    }

    public void OnSettingsExit()
    {
        gameObject.SetActive(true);
    }
}
