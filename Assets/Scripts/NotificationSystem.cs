using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NotificationSystem : MonoBehaviour
{
    [SerializeField] private GameObject notificationPrefab;

    private List<CustomNotif> queuedNotifications;

    private float _baseY = -50;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        queuedNotifications = new List<CustomNotif>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckNotifications();
    }

    public void AddNotification(string message, string type, string color)
    {
        GameObject notif = Instantiate(notificationPrefab, transform);

        CustomNotif notifComp = notif.GetComponent<CustomNotif>();

        notifComp.UpdateContents(message, type, color);

        queuedNotifications.Add(notifComp);

        notifComp.createdTime = Time.time;

        RectTransform rt = notif.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, _baseY - 55 * (queuedNotifications.Count - 1));
    }

    private void CheckNotifications()
    {
        List<CustomNotif> actualNotifs = queuedNotifications.ToList();

        foreach (CustomNotif notif in actualNotifs)
        {
            if (Time.time - notif.createdTime > 5)
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
