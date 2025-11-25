using UnityEngine.Events;

public static class UIEvents
{
    public static UnityEvent OnMainMenuEnter = new UnityEvent();
    public static UnityEvent OnMainMenuExit = new UnityEvent();

    public static UnityEvent OnPlayEnter = new UnityEvent();

    public static UnityEvent LoadedListeners = new UnityEvent();

    public static UnityEvent OnTransitionExit = new UnityEvent();
}