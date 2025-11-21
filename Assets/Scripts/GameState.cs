public abstract class GameState
{
    protected GameMaster _gm;

    protected GameState(GameMaster gm)
    {
        _gm = gm;
    }

    public abstract void OnStateEnter();

    public abstract void OnStateExit();
}

public class MainMenuState : GameState
{
    public MainMenuState(GameMaster gm) : base(gm) { }

    public override void OnStateEnter()
    {
        UIEvents.OnMainMenuEnter?.Invoke();
    }

    public override void OnStateExit()
    {
        UIEvents.OnMainMenuExit?.Invoke();
    }
}


public class PlayState : GameState
{
    public PlayState(GameMaster gm) : base(gm) { }

    public override void OnStateEnter()
    {
        UIEvents.OnPlayEnter?.Invoke();

        // Load level
        GameEvents.OnPlayEnter?.Invoke();
    }

    public override void OnStateExit()
    {
        GameEvents.OnPlayExit?.Invoke();
    }
}


public class InHangarState : GameState
{
    public InHangarState(GameMaster gm) : base(gm) { }

    public override void OnStateEnter()
    {

    }

    public override void OnStateExit()
    {

    }
}


public class EndState : GameState
{
    public EndState(GameMaster gm) : base(gm) { }

    public override void OnStateEnter()
    {
        // Transitioned by another state
    }

    public override void OnStateExit()
    // Catch listener (main menu or restart)
    {
        // Go to Main Menu

        // Restart game
    }
}