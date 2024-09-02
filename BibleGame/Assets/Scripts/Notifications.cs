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


    string currentTitle = string.Empty;
    string currentMessage = string.Empty;   

    private void Awake()
    {
        if (instance == null)
            instance = this;

        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
    }

    void AskNotification()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionDenied += OnPermissionDenied;
            callbacks.PermissionGranted += OnPermissionGranted;
            callbacks.PermissionDeniedAndDontAskAgain += OnPermissionDeniedAndDontAskAgain;

            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS", callbacks);
        }
        else
        {
            Debug.Log("Permission was grnated");

            var channel = new AndroidNotificationChannel()
            {
                Id = "channel_id",
                Name = "Default Channel",
                Importance = Importance.Default,
                Description = "Generic notyification",
            };

            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            var notification = new AndroidNotification();
            notification.Title = currentTitle;
            notification.Text = currentMessage;
            notification.SmallIcon = "icon";
            notification.LargeIcon = "logo";

            notification.FireTime = System.DateTime.Now;
            AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }
    }

    void OnPermissionDenied(string permissionName)
    {
        Debug.Log($"{permissionName} permission denied");
    }

    void OnPermissionGranted(string permissionName)
    {
        Debug.Log($"{permissionName} permission granted");

        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notyification",
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        var notification = new AndroidNotification();
        notification.Title = currentTitle;
        notification.Text = currentMessage;
        notification.SmallIcon = "icon";
        notification.LargeIcon = "logo";

        notification.FireTime = System.DateTime.Now;
        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }

    void OnPermissionDeniedAndDontAskAgain(string permissionName)
    {
        Debug.Log($"{permissionName} permission denied and don't ask again");
    }

    public void SendNotification(string title, string message)
    {
        currentTitle = title;
        currentMessage = message;
        
        AskNotification();
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
