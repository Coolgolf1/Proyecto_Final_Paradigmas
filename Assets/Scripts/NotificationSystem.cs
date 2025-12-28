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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        queuedNotifications = new List<CustomNotif>();
        source = GetComponent<AudioSource>();
        source.clip = popSound;
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
}
