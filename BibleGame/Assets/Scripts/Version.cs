using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using BibleGame.API;
using BibleGame;
using DebugUtils;

[RequireComponent(typeof(TMP_Text))]
public class Version : MonoBehaviour
{
    TMP_Text mText;

    private void Awake()
    {
        mText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        mText.text = $"ver {Application.version}";

        Actions.VersionCheck += VersionCheck;
    }

    private void OnDisable()
    {
        Actions.VersionCheck -= VersionCheck;
    }

    public void VersionCheck()
    {
        VersionAPI.GetVersion((success, res) =>
        {
            if (!success)
                return;

            DevDebug.Log($"Current Ver : {Application.version} :: {res.ResponseData.android_latest_version}", DebugColor.Grey);

#if UNITY_ANDROID
     if(Application.version != res.ResponseData.android_latest_version)
            {
                PopUp.Instance.ShowMessage("Your Application is outdated.Please do update the application");
            }
#else
            if(Application.version != res.ResponseData.ios_latest_version)
            {
                PopUp.Instance.ShowMessage("Your Application is outdated.Please do update the application");
            }
#endif

        });
    }
}
