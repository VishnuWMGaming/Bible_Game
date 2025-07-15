using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using BibleGame;

public class LevelPanel : MonoBehaviour,IChapterButton ,ICover
{
    [Header("UI Settings:")]
    [SerializeField] Button settingsBtn;
    [SerializeField] Button backBtn;


    [Header("Level Buttons:")]
    [SerializeField] List<Chapter_Button> level_Buttons = new List<Chapter_Button>();

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
