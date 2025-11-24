using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using DebugUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StrickPanel : MonoBehaviour
{
    [SerializeField] Button streak1Btn;
    [SerializeField] Button streak2Btn;
    [SerializeField] Button newGame;

    [SerializeField] Button homeBtn;

    [SerializeField] TMP_Text userName;

    [Space]
    [SerializeField] PostImage iPic;


    private void OnEnable()
    {
        newGame?.onClick.AddListener(NewGame);
        homeBtn?.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.home); });

        streak1Btn?.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); GetStreak(0); });
        streak2Btn?.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); GetStreak(StreakAPI.streakDatas.Count -1); });

        streak1Btn.gameObject.SetActive(StreakAPI.streakDatas.Count >= 1);
        streak2Btn.gameObject.SetActive(StreakAPI.streakDatas.Count >= 2);

        userName.text = AppData.loginData.Name;

        if(AppData.loginData.Pic != null)
        iPic.SetRightSize(AppData.loginData.Pic, true);

    }

    private void OnDisable()
    {
        streak1Btn?.onClick.RemoveAllListeners();
        streak2Btn?.onClick.RemoveAllListeners();
        newGame?.onClick.RemoveAllListeners();
        homeBtn?.onClick.RemoveAllListeners();
    }


    void NewGame()
    {
        Actions.ChangePanelActions(CanvasType.ageSelect);
    }

    void GetStreak(int index)
    {
        if (StreakAPI.streakDatas == null || StreakAPI.streakDatas.Count == 0)
            return;

        StreakData streakData = StreakAPI.streakDatas[index];

        UserData.gameid = streakData._id;
        UserData.bookId = streakData.book_id;
        UserData.bibleId = streakData.bible_id;

        PopUp.Instance.EnableLoad(true);
        GetBiblesAPI.GetDetail((success, res) =>
        {
            PopUp.Instance.EnableLoad(false);

            if (!success)
            {
                Debug.LogError("Error in getting bible details");
            }

            DevDebug.Log($"Bible Name: {res.ResponseData.data.nameLocal}", DebugColor.Green);

            UserData.bibleName = res.ResponseData.data.nameLocal;

            UserData.currentAge = streakData.age switch
            {
                "1" => AgeGroup.kindergarden,
                "2" => AgeGroup.elementary,
                "3" => AgeGroup.teenagers,
                "4" => AgeGroup.adult,
                _ => AgeGroup.elementary
            };

            UserData.testament = streakData.testament switch
            {
                "Old" => Testament.Old,
                "New" => Testament.New,
                _ => throw new System.NotImplementedException(),
            };

            Debug.Log($"<color=cyan> testamant: {UserData.testament} </color>");

            UserData.coins = streakData.coins;

            Actions.ChangePanelActions(CanvasType.level);

        }, UserData.bibleId);
       
    }
}