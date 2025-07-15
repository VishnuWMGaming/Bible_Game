using BibleGame;
using BibleGame.API;
using BibleGame.Data;
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


    private void OnEnable()
    {
        newGame?.onClick.AddListener(NewGame);
        homeBtn?.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.home));

        streak1Btn?.onClick.AddListener(() => GetStreak(1));
        streak2Btn?.onClick.AddListener(() => GetStreak(2));

        userName.text = AppData.loginData.Name;
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
        UserData.currentAge = streakData.age switch
        {
            "1" => AgeGroup.kindergarden,
            "2" => AgeGroup.elementary,
            "3" => AgeGroup.teenagers,
            "4" => AgeGroup.adult,
            _=> AgeGroup.elementary
        };

        UserData.testament = streakData.testament switch
        {
            "Old" => Testament.Old,
            "New" => Testament.New,
            _ => throw new System.NotImplementedException(),
        };

        UserData.coins = streakData.coins;

        if (String.IsNullOrEmpty(UserData.gameid))
        {
            Debug.LogError("Streak api id is null");
            return;
        }

        PopUp.Instance.EnableLoad(true);
        StreakAPI.GetDetail((success, res) =>
        {
            PopUp.Instance.EnableLoad(false);
            if (!success)
            {
                Debug.LogError("Error in getting details");
                return;
            }





        }, UserData.gameid);
    }
}

