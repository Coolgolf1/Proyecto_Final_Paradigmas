public abstract class GameState
{
    public MainMenuState mainMenu;
    public PlayState play;
    public InHangarState inHangar;
    public EndState end;

    public abstract void OnStateEnter();

    public abstract void OnStateExit();
}

public class MainMenuState : GameState
{
    public override void OnStateEnter()
    {
        // Transitioned by another state
    }

    public override void OnStateExit()
    {
        // Catch listener (start button)
    }
}

public class PlayState : GameState
{
    public override void OnStateEnter()
    {
        // Transitioned by another state
    }

    public override void OnStateExit()
    {
        // Catch listener (airport capacity boom)
    }
}

public class InHangarState : GameState
{
    public override void OnStateEnter()
    {
        // Transitioned by another state
    }

    public override void OnStateExit()
    {
        // Catch listener
    }
}

public class EndState : GameState
{
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