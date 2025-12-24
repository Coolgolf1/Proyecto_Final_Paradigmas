using UnityEngine.Events;

public static class UIEvents
{
    public static UnityEvent OnMainMenuEnter = new UnityEvent();
    public static UnityEvent OnMainMenuExit = new UnityEvent();

    public static UnityEvent OnPlayEnter = new UnityEvent();

    public static UnityEvent LoadedListeners = new UnityEvent();

    public static UnityEvent OnTransitionExit = new UnityEvent();

    public static UnityEvent OnAirplaneStoreEnter = new UnityEvent();
    public static UnityEvent OnAirplaneStoreExit = new UnityEvent();

    public static UnityEvent OnRouteStoreEnter = new UnityEvent();
    public static UnityEvent OnRouteStoreExit = new UnityEvent();

    public static UnityEvent OnEndGameEnter = new UnityEvent();
    public static UnityEvent OnEndGameExit = new UnityEvent();

    public static UnityEvent OnRestartGame = new UnityEvent();
}