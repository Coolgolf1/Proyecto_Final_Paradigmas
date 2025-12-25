using UnityEngine.Events;

public static class GameEvents
{
    public static UnityEvent OnPlayEnter = new UnityEvent();
    public static UnityEvent OnPlayExit = new UnityEvent();

    public static UnityEvent OnMainMenuEnter = new UnityEvent();
    public static UnityEvent OnMainMenuExit = new UnityEvent();

    public static UnityEvent OnTransitionExit = new UnityEvent();

    public static UnityEvent OnAirportUnlock = new UnityEvent();

    public static UnityEvent OnPlaneLandedAndBoarded = new UnityEvent();
}
