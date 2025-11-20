using UnityEngine;

public class GameMaster : MonoBehaviour
{
    private GameState _currentState;

    public MainMenuState MainMenu { get; private set; }
    public PlayState Play { get; private set; }
    public InHangarState InHangar { get; private set; }
    public EndState End { get; private set; }

    void Awake()
    {
        MainMenu = new MainMenuState(this);
        Play = new PlayState(this);
        InHangar = new InHangarState(this);
        End = new EndState(this);
    }

    void Start()
    {
        ChangeState(MainMenu);
    }

    public void ChangeState(GameState nextState)
    {
        _currentState?.OnStateExit();
        _currentState = nextState;
        _currentState?.OnStateEnter();
    }
}
