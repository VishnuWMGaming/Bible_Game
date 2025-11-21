using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using DebugUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour,IChapterButton ,ICover,ILevelObj
{
    [Header("UI Settings:")]
    [SerializeField] Button settingsBtn;
    [SerializeField] Button backBtn;

    [SerializeField] TMP_Text scoreText;

    [Header("Cells:")]
    [SerializeField] List<Cell> cells = new List<Cell>();

    [Header("Chapters")]
    [SerializeField] List<ChapterData> chapters = new List<ChapterData>();
 
    [Space]
    [SerializeField] Button mPreviousBtn;
    [SerializeField] Button mNextBtn;

    [SerializeField] GameObject splashObj;

    [Header("LevelScreen:")]
    [SerializeField] GameObject mLevelObj;


    [SerializeField] int mFirstIndex = 0;
    [SerializeField] int mLastIndex = 0;

    [SerializeField] List<LevelData> levelDatas;


    private void OnEnable()
    {
        scoreText.text = UserData.coins.ToString();

        Initialise();

        settingsBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.setPanel); });
        backBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.home); });

        splashObj.SetActive(false);
        mLevelObj.SetActive(true);

        mPreviousBtn.interactable = false;
        mNextBtn.interactable = true;

        mPreviousBtn.onClick.AddListener(PreviousCell);
        mNextBtn.onClick.AddListener(NextCell);

        cells[0].gameObject.SetActive(false);
        cells[1].gameObject.SetActive(false);
        cells[2].gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        settingsBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();

        mPreviousBtn.onClick.RemoveAllListeners();
        mNextBtn.onClick.RemoveAllListeners();

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

            levelDatas = res.ResponseData.levels;

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

                chapters = GameData.mChapterDatas;

                //for(int i = 0; i< levelBtns.Count;++i)
                //{
                //    levelBtns[i].Intialise(GameData.mChapterDatas[i].id, this);
                //}

                mFirstIndex = 0;
                mLastIndex = cells[0].LevelObjs.Count - 1;

                if (mLastIndex >= GameData.mChapterDatas.Count)
                {
                    mLastIndex = GameData.mChapterDatas.Count - 1;
                    mNextBtn.interactable = false;
                }

                List<ChapterData> chapterDatas = GameData.mChapterDatas
                                             .Skip(mFirstIndex)
                                             .Take(mLastIndex - mFirstIndex + 1)
                                             .ToList();

                cells[0].gameObject.SetActive(true);
                cells[0].Init(chapterDatas, this);

                int coins = 0;

                if (levelDatas != null || levelDatas.Count != 0)
                 foreach (var item in levelDatas)
                 {
                     coins += item.coin_earn;
                 }

                scoreText.text = coins.ToString();

                if (levelDatas != null || levelDatas.Count != 0)
                    foreach (var level in levelDatas)
                    {
                        //DevDebug.Log($"Level Update:{level.chapter_id} :: {level.coin_earn}", DebugColor.Turquoise);
                        if (level.coin_earn >= 20)
                            cells[0].Get(level.chapter_id).Finish(true);
                    }

            }, UserData.bibleId, UserData.bookId);


        }, UserData.gameid);
    }


    #region NAVIGATION

    void PreviousCell()
    {
        if (mFirstIndex < 0)
            return;

        int activeIndex = cells.FindIndex(obj => obj.gameObject.activeInHierarchy);

        activeIndex--;
       
        if(activeIndex < 0)
            activeIndex = 2;

        mLastIndex = mFirstIndex-1;
        mFirstIndex = mFirstIndex - cells[activeIndex].LevelObjs.Count;

        if (mFirstIndex < 0)
        {
            mFirstIndex = 0;
            return;
        }

        List<ChapterData> chapterDatas = GameData.mChapterDatas
                                                .Skip(mFirstIndex)
                                                .Take(mLastIndex - mFirstIndex + 1)
                                                .ToList();

        foreach (var cell in cells)
            cell.gameObject.SetActive(false);
 
        cells[activeIndex].gameObject.SetActive(true);
        cells[activeIndex].Init(chapterDatas, this);

        mPreviousBtn.interactable = mFirstIndex > 0;
        mNextBtn.interactable = true;

        if (levelDatas != null || levelDatas.Count != 0)
            foreach (var level in levelDatas)
            {
                DevDebug.Log($"Level Update:{level.chapter_id} :: {level.coin_earn}", DebugColor.Turquoise);

                if (level.coin_earn >= 20)
                   cells[activeIndex]?.Get(level.chapter_id)?.Finish(true);
            }
    }

    void NextCell()
    {
        if (mLastIndex >= GameData.mChapterDatas.Count)
            return;

        int activeIndex = cells.FindIndex(obj => obj.gameObject.activeInHierarchy);

        activeIndex++;
        if (activeIndex > cells.Count - 1)
            activeIndex = 0;

        mFirstIndex = mLastIndex +1;
        mLastIndex = mFirstIndex + (cells[activeIndex].LevelObjs.Count - 1);

        if(mLastIndex >= GameData.mChapterDatas.Count)
            mLastIndex = GameData.mChapterDatas.Count - 1;

        List<ChapterData> chapterDatas = GameData.mChapterDatas
                                                 .Skip(mFirstIndex)
                                                 .Take(mLastIndex - mFirstIndex + 1)
                                                 .ToList();
        foreach(var cell in cells) 
            cell.gameObject.SetActive(false);

        cells[activeIndex].gameObject.SetActive(true);
        cells[activeIndex].Init(chapterDatas, this);

       // mFirstIndex = mLastIndex;
       // mLastIndex = cells[activeIndex].LevelObjs.Count;

        mPreviousBtn.interactable = true;
        mNextBtn.interactable = mLastIndex +1  < GameData.mChapterDatas.Count;

        if (levelDatas != null || levelDatas.Count != 0)
            foreach (var level in levelDatas)
            {
               DevDebug.Log($"Level Update:{level.chapter_id} :: {level.coin_earn}", DebugColor.Turquoise);

                if(level.coin_earn >=20)
                cells[activeIndex]?.Get(level.chapter_id)?.Finish(true);
            }
    }

    #endregion

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
