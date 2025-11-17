using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] private Button startGame;
    [SerializeField] private Button options;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject earth;

    private bool _startAnimation;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startGame.onClick.AddListener(StartGame);
        earth.GetComponent<SpinFree>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (_startAnimation)
        {
            
            player.transform.position = Vector3.Lerp(player.transform.position, GameConstants.initCameraPosition, 3* Time.deltaTime);

            if ((GameConstants.initCameraPosition - player.transform.position).magnitude < 0.1)
            {
                player.transform.position = GameConstants.initCameraPosition;
                _startAnimation = false;
                gameObject.SetActive(false);
                earth.GetComponent<SpinFree>().enabled = false;
            }
        }

        //if (player.transform.position == GameConstants.initCameraPosition) 
        //    _startAnimation = false;
        
    }

    void StartGame()
    {
        _startAnimation = true;
    }
}
