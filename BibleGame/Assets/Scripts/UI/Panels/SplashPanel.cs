using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using RestAPI;
using UnityEngine.Serialization;

public class SplashPanel : MonoBehaviour
{
    [FormerlySerializedAs("button")]
    [Header("UI Settings:")]
    [SerializeField] Button continueButton;

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnEnable()
    {
        if (PlayerPrefs.HasKey("AuthorizationToken"))
        {
            SetAuthToken.SetToken(PlayerPrefs.GetString("AuthorizationToken"));
            Invoke("GetProfile", .5f);
        }

        continueButton.onClick.AddListener(() =>
        {
            if (PlayerPrefs.HasKey("AuthorizationToken"))
            {
                PopUp.Instance.EnableLoad(true);
                Invoke("OpenHome", 2);
            }
            else
            {
                Actions.ChangePanelActions(CanvasType.login);
            }

            AudioManager.Instance.PlayButton();
        });
    }
    
    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        continueButton.onClick.RemoveAllListeners();
    }

    private void GetProfile()
    {
        GetProfileAPI.GetProfile(GetProfileCallback);
    }

    private void OpenHome()
    {
       // PopUp.Instance.EnableLoad(false);
        Actions.ChangePanelActions(CanvasType.home);
    }

    private void GetProfileCallback(bool success, GetProfileResponse response)
    {
        if (success)
        {
            AppData.loginData = new LoginData(response.ResponseData.email, "**********", response.ResponseData.name,response.ResponseData.church);
          //  GetChapters();
        }
    }

  

    private void GetChaptersCallback(bool success, GetChaptersResponse response)
    {
        if (success)
        {
            //List<Chapter> chapters = new List<Chapter>();
            //foreach (var chapter in response.ResponseData)
            //{
            //    Chapter obj = new Chapter(chapters.Count, chapter.id, chapter.name, chapter.description);
            //    chapters.Add(obj);
            //}
            
           // GameData.SetChapters(chapters);
        }
    }
}


public class SetAuthToken : ApiBase
{
    public static void SetToken(string token)
    {
        SetAuthToken(token);
    }
}
