#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

  
[InitializeOnLoad]
public class PushAlertSystem
{
    private static string timeSetKey = "LastDialogue";
    private static string audioPathName = "Packages/com.webmobril.core/Audio/takemergeAudio.mp3"; 
 
    static PushAlertSystem()
    {
#if SHOWMERGEALERT
 
        string lastDateTime = PlayerPrefs.GetString(timeSetKey, string.Empty);
        Debug.LogFormat("LastDateTime {0}", lastDateTime);
        if (!string.IsNullOrEmpty(lastDateTime))
        {
            DateTime lastTime = Convert.ToDateTime(lastDateTime);

            int dayDifference = DateTime.Now.Day - lastTime.Day; 
             
            Debug.Log(dayDifference);
            if (dayDifference > 0 && DateTime.Now.Hour > 10)  
            {  
                ShowDialogue();
            }

            UpdateLastShownTime();
        }
        else
        {
            ShowDialogue();
            UpdateLastShownTime();
        }

     //   ShowDialogue();
        
#endif
    }

    private static void UpdateLastShownTime()
    {
        PlayerPrefs.SetString(timeSetKey, DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    private static void ShowDialogue()
    {
        AudioClip clip = (AudioClip) EditorGUIUtility.Load(audioPathName);
        PlayClip(clip);
        EditorUtility.DisplayDialog("Merge From Main",
            "Take Merge From Main Before Starting Work", "Done");
    }

    private static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "PlayPreviewClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] {typeof(AudioClip), typeof(int), typeof(bool)},
            null 
        );

        // Debug.Log(method);
        method.Invoke(
            null,
            new object[] {clip, startSample, loop});
    }
}
#endif