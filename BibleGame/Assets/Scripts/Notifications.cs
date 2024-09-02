using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using UnityEngine.Android;
using UnityEngine.Events;

public class Notifications : MonoBehaviour
{
    static Notifications instance;

    public static Notifications Instance { get { return instance; } }

    public UnityEvent Requestcallback;

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

        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            Requestcallback?.Invoke();
            return;
        }


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

    bool IsNotificationsEnabled()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject notificationManagerCompat = new AndroidJavaObject("androidx.core.app.NotificationManagerCompat", activity);
        
        bool notificationsEnabled = notificationManagerCompat.Call<bool>("areNotificationsEnabled");
        return notificationsEnabled;
#else
        // For other platforms or editor, return true as a fallback
        return true;
#endif
    }

}
