using UnityEngine;

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
        GameEvents.OnMainMenuEnter?.Invoke();
    }

    public override void OnStateExit()
    {
        UIEvents.OnMainMenuExit?.Invoke();
        GameEvents.OnMainMenuExit?.Invoke();
    }
}

public class MainTransitionGameState : GameState
{
    public MainTransitionGameState(GameMaster gm) : base(gm) { }

    public override void OnStateEnter()
    {
        //UIEvents.OnMainMenuExit?.Invoke();
    }

    public override void OnStateExit()
    {
        UIEvents.OnTransitionExit?.Invoke();
        GameEvents.OnTransitionExit?.Invoke();
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
        Time.timeScale = 0;
        UIEvents.OnEndGameEnter?.Invoke();
    }

    public override void OnStateExit()
    {
        Time.timeScale = 1;
        UIEvents.OnEndGameExit?.Invoke();
    }
}