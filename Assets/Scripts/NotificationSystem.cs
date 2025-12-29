using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class NotificationSystem : MonoBehaviour
{
    [SerializeField] private GameObject notificationPrefab;

    private List<CustomNotif> queuedNotifications;

    private AudioSource source;
    [SerializeField] private AudioClip popSound;

    private float _baseY = -50;

    private bool _hidden = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        queuedNotifications = new List<CustomNotif>();
        source = GetComponent<AudioSource>();
        source.clip = popSound;
        UIEvents.OnMainMenuEnter.AddListener(ClearAllNotifications);
        UIEvents.OnSettingsEnter.AddListener(ClearAllNotifications);
        UIEvents.OnAirplaneStoreEnter.AddListener(HideNotifications);
        UIEvents.OnRouteStoreEnter.AddListener(HideNotifications);
        UIEvents.OnAirplaneStoreExit.AddListener(ShowNotifications);
        UIEvents.OnRouteStoreExit.AddListener(ShowNotifications);
    }

    // Update is called once per frame
    void Update()
    {
        CheckNotifications();
    }

    public void AddNotification(string message, string type, string color, UnityAction onClickFunc = null)
    {
        GameObject notif = Instantiate(notificationPrefab, transform);

        CustomNotif notifComp = notif.GetComponent<CustomNotif>();

        notifComp.UpdateContents(message, type, color, onClickFunc);

        queuedNotifications.Add(notifComp);

        notifComp.createdTime = Time.fixedTime;

        RectTransform rt = notif.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, _baseY - 55 * (queuedNotifications.Count - 1));

        source.Play();

        if (_hidden)
        {
            notifComp.SetHidden();
        }
    }

    private void CheckNotifications()
    {
        List<CustomNotif> actualNotifs = queuedNotifications.ToList();

        foreach (CustomNotif notif in actualNotifs)
        {
            if (Time.fixedTime - notif.createdTime > 10)
            {
                queuedNotifications.Remove(notif);
                notif.FadeOutAndDestroy();
            }
        }

        for (int i = 0; i < queuedNotifications.Count; i++)
        {
            queuedNotifications[i].objY = _baseY - 55 * i;
        }
    }

    public void ClearAllNotifications()
    {
        List<CustomNotif> actualNotifs = queuedNotifications.ToList();

        foreach (CustomNotif notif in actualNotifs)
        {
            
            queuedNotifications.Remove(notif);
            notif.FadeOutAndDestroy();
            
        }
    }

    public void HideNotifications()
    {
        _hidden = true;
        foreach (CustomNotif notif in queuedNotifications)
        {
            notif.SetHidden();
        }
    }

    public void ShowNotifications()
    {
        _hidden = false;
        foreach (CustomNotif notif in queuedNotifications)
        {
            notif.SetShow();
        }
    }
}
