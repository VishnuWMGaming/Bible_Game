using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using BibleGame.Utility;
using DebugUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class HomePanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] TMP_Text userName;
    [SerializeField] TMP_Text churchName;

    [SerializeField] Button settingBtn;
    [SerializeField] Button playBtn;
    [SerializeField] Button leaderBoardBtn;

    [Space]
    [SerializeField] PostImage iPic;

    [Header("ImageDownloader")]
    [SerializeField] ImageDownloader imageDownloader;

    /// <summary>
    /// Action implemented on enable
    /// </summary>
    private void OnEnable()
    {
        settingBtn?.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.setPanel); });

        //playBtn.interactable = false;

        AudioManager.Instance.PlayBG(AudioType.Bg);

        //  Leaderboard button listener
        leaderBoardBtn?.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayButton();
            Actions.ChangePanelActions(CanvasType.leaderboard); // match enum
        });


        PopUp.Instance.EnableLoad(true);
        Initialise();
    }

    async void Initialise()
    {
        GetProfileAPI.GetProfile(async (success,res) =>
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

            Sprite pic = res.ResponseData.profile_pic == "0" ? null  : await DownloadSpriteAsync($"{ServiceURL.imageURL}{res.ResponseData.profile_pic}");

            if(pic != null)
            iPic.SetRightSize(pic, true);

            AppData.loginData = new LoginData(AppData.loginData.Email, AppData.loginData.Password, AppData.loginData.Name, res.ResponseData.church,pic);

            #region Fetching streak details


            PopUp.Instance.EnableLoad(true);
            DevDebug.Log("Fetching the streak details", DebugColor.Silver);

            StreakAPI.Get((success, res) =>
            {
                PopUp.Instance.EnableLoad(false);
                DevDebug.Log("Streak details fetched", DebugColor.Green);

                // playBtn.interactable = true;

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
            #endregion
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


    public async Task<Sprite> DownloadSpriteAsync(string url)
    {
        var tcs = new TaskCompletionSource<Sprite>();

        Debug.Log($"<color=cyan>Image URL: {url}</color>");

        imageDownloader.DownloadImage(url, (tex) =>
        {
            if (tex != null)
            {
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                tcs.SetResult(sprite);
            }
            else
            {
                tcs.SetResult(null);
            }
        });

        return await tcs.Task;
    }
}