using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using BibleGame;
using BibleGame.Data;
using BibleGame.API;
public class HomePanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] TMP_Text userName;
    [SerializeField] TMP_Text churchName;

    [SerializeField] Button settingBtn;
    [SerializeField] Button playBtn;
    [SerializeField] Button leaderBoardBtn;

    /// <summary>
    /// Action implemented on enable
    /// </summary>
    private void OnEnable()
    {
        settingBtn?.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.setPanel); });

        playBtn.interactable = false;

        //  Leaderboard button listener
        leaderBoardBtn?.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayButton();
            Actions.ChangePanelActions(CanvasType.leaderboard); // match enum
        });

        Initialise();
    }

    void Initialise()
    {
        PopUp.Instance.EnableLoad(true);
        GetProfileAPI.GetProfile((success,res) =>
        {
            PopUp.Instance.EnableLoad(false);

            if (!success)
            {
                Debug.LogError("Unable to fetch the profile");
                PopUp.Instance.ShowMessage("Unable to fetch the profile ");
                return;
            }

            userName.text = res.ResponseData.name;
            churchName.text = $"Church: {res.ResponseData.church}";

            AppData.loginData = new LoginData(AppData.loginData.Email, AppData.loginData.Password, AppData.loginData.Name, res.ResponseData.church);
        });

        PopUp.Instance.EnableLoad(true);

        StreakAPI.Get((success, res) =>
        {
            PopUp.Instance.EnableLoad(false);
            playBtn.interactable = true;

            if (!success)
            {
                Debug.LogError("Error in getting the streaks");
                playBtn?.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.ageSelect));

                return;
            }

            playBtn?.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlayButton();

                if (res.ResponseData == null || res.ResponseData.Count <= 0)
                    Actions.ChangePanelActions(CanvasType.ageSelect);
                else
                    Actions.ChangePanelActions(CanvasType.selectStreak);
            });

        });
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        settingBtn?.onClick.RemoveAllListeners();
        playBtn?.onClick.RemoveAllListeners();
        leaderBoardBtn.onClick.RemoveAllListeners();
    }
}