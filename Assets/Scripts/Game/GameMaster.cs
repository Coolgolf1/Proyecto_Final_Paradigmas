
public class GameMaster
{
    private static GameMaster _instance;

    public GameState currentState;



    public MainMenuState MainMenu { get; private set; }
    public PlayState Play { get; private set; }
    public MainTransitionGameState Transition { get; private set; }
    public InHangarState InHangar { get; private set; }
    public EndState End { get; private set; }

    private GameMaster()
    {
        MainMenu = new MainMenuState(this);
        Play = new PlayState(this);
        Transition = new MainTransitionGameState(this);
        InHangar = new InHangarState(this);
        End = new EndState(this);

        // ARREGLAR ESTO EN OTRO SITIO ============================================
        DijkstraGraph.Initialise();

        UIEvents.LoadedListeners.AddListener(InitState);
    }

    public void InitState()
    {
        ChangeState(MainMenu);
    }

    public static GameMaster GetInstance()
    {
        if (_instance is null)
        {
            _instance = new GameMaster();
        }

        return _instance;
    }

    public void ChangeState(GameState nextState)
    {
        currentState?.OnStateExit();
        currentState = nextState;
        currentState?.OnStateEnter();
    }
}
