using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour,IChapterButton ,ICover,ILevelObj
{
    [Header("UI Settings:")]
    [SerializeField] Button settingsBtn;
    [SerializeField] Button backBtn;

    [SerializeField] TMP_Text scoreText;


    [Header("Level Buttons:")]
    [SerializeField] List<LevelObj> levelBtns = new List<LevelObj>();

    [SerializeField] GameObject splashObj;

    [Header("LevelScreen:")]
    [SerializeField] GameObject mLevelObj;

    private void OnEnable()
    {
        //for (int i = 0; i < level_Buttons.Count; i++)
        //    level_Buttons[i].callback = this;
        //Debug.Log("GameData.GetChapters().Count: " + GameData.GetChapters().Count);
        //for (int i = 0; i < GameData.GetChapters().Count; i++)
        //{
        //    level_Buttons[i].ChapterID = GameData.GetChapters()[i].chapterID;
        //    level_Buttons[i].Level = GameData.GetChapters()[i].chapterIndex;
        //}

        scoreText.text = UserData.coins.ToString();

        Initialise();

        settingsBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.setPanel));
        backBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.home));

        splashObj.SetActive(false);
        mLevelObj.SetActive(true);
    }

    private void OnDisable()
    {
        settingsBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();

        splashObj.SetActive(true);
        mLevelObj.SetActive(false);
    }

    void Initialise()
    {
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

            Debug.LogWarning("Got the streak details !!");

            UserData.bookId = res.ResponseData.streak.book_id;

            int coins = 0;

            List<LevelData> levelDatas = res.ResponseData.levels;

            if(String.IsNullOrEmpty(res.ResponseData.streak.book_id) || String.IsNullOrEmpty(res.ResponseData.streak.bible_id))
            {
                Debug.LogError("ids are null for the chapter call");
                return;
            }

            PopUp.Instance.EnableLoad(true);
            ChapterAPI.Get((success, res) =>
            {
                PopUp.Instance.EnableLoad(false);

                if (!success)
                {
                    Debug.LogError("Error in getting the chapters");
                    return;
                }

                GameData.mChapterDatas = res.ResponseData.data;

                for(int i = 0; i< levelBtns.Count;++i)
                {
                    levelBtns[i].Intialise(GameData.mChapterDatas[i].id, this);
                }

                int coins = 0;

                if (levelDatas != null || levelDatas.Count != 0)
                 foreach (var item in levelDatas)
                 {
                     coins += item.coin_earn;
                 }

                scoreText.text = coins.ToString();

               if (levelDatas != null || levelDatas.Count != 0)
                foreach(var level in levelDatas)
                {
                    levelBtns.Find(x => x.ID == level.chapter_id).StarUpdate(level.rating);
                }

            }, UserData.bibleId, UserData.bookId);


        }, UserData.gameid);
    }

    public void ChapterSelect(string id)
    {
        Debug.Log($"<color=green> Chapter selected : {id}</color>");

        UserData.chapterId = id;

        Actions.ChangePanelActions(CanvasType.chapter);
        Actions.StartPageAction(StartPage.chapter);
    }

    /// <summary>
    /// Action implemented on level selection
    /// </summary>
    /// <param name="level"></param>
    public void OnLevelSelectAction()
    {
        Actions.ChangePanelActions(CanvasType.chapter);
    }

    /// <summary>
    /// Action implemented on select chapter
    /// </summary>
    /// <param name="chapter"></param>
    public void SelectChapter(string chapter)
    {
        
    }
}
