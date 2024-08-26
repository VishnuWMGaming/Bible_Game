using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using UnityEngine.Android;

public class Notifications : MonoBehaviour
{
    static Notifications instance;

    public static Notifications Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
    }

   
    public void SendNotification(string title, string message)
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notyification",
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = message;
        notification.SmallIcon = "icon";
        notification.LargeIcon = "logo";

        notification.FireTime = System.DateTime.Now;
        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }

}
