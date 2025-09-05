using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using System;
using BibleGame.Data;
using BibleGame.API;


using BibleGame.Utility;

public interface IBookChapter
{
    public void EndBookAction();

    public void BackToCover();

    public void BackToBooks();
}

public class BookChapter : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] Button previousBtn;
    [SerializeField] Button nextBtn;
    [SerializeField] Button backBtn;

    [SerializeField] TMP_Text header;

    [Header("Pages:")]
    [SerializeField] List<PagePanel> pagePanels = new List<PagePanel>();

    //private Chapter chapter;

    [SerializeField] private TextMeshProUGUI pagePanel;
    [SerializeField] ScrollRect mScorll;


    enum Navigate { previous , next };


    public IBookChapter callback;

    int _currentPageIndex = 0;
    int pageCount = 0;

    [Header("SpriteData")]
    [SerializeField] SpriteData spriteData;

  

    /// <summary>
    /// Action imeplemented on enable
    /// </summary>
    private void OnEnable()
    {
       // previousBtn.onClick.AddListener(() => PageNavigateAction(Navigate.previous));
        nextBtn.onClick.AddListener(()=> callback.EndBookAction());

        GetChapter();

       // chapter = GameData.GetDataWithChapterID(GameDat);
       // header.text = chapter.chapterName;

        StyleUI styleUI = spriteData.GetStyle(UserData.currentAge);

        pagePanel.color = styleUI.textColor;

        //int pageCount = Mathf.CeilToInt(chapter.chapterDescription.Length / 100);
        //pageCount++;



        // for (int i = 0; i < pagePanels.Count; ++i)
        //     pagePanels[i].gameObject.SetActive(false);
        //
        // pagePanels[0].gameObject.SetActive(true);
        // header.text = "Chapter " + pagePanels.Find(x => x.gameObject.activeInHierarchy).Page.chapterIndex;

        previousBtn.interactable = false;
    }

    private void OnDisable()
    {
        previousBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.RemoveAllListeners();
    }


    void GetChapter()
    {
        //if (index > pageCount)
        //{
        //    callback.EndBookAction();
        //    return;
        //}

        //if (index <= 1) index = 1;

        PopUp.Instance.EnableLoad(true);
        ChapterAPI.GetDetail((success, res) =>
        {
            PopUp.Instance.EnableLoad(false);
            if (!success)
            {
                Debug.LogError("Error in getting chapter details");
                return;
            }

            mScorll.verticalNormalizedPosition = 1.0f;

            header.text =  $"Chapter {res.ResponseData.data.number}";

            Debug.Log($"<color=green> Chapter {res.ResponseData.data.id} is loaded.</color>");

            string content = res.ResponseData.data.content.ToString();
            content = Utils.ConvertHtmlToPlainText(content);

            pagePanel.text = content;

        }, UserData.bibleId, UserData.chapterId);
    }


    //void PageNavigateAction(Navigate navigate)
    //{
    //    switch (navigate)
    //    {
    //        case Navigate.next :

    //            _currentPageIndex++;
    //            GetChapter(_currentPageIndex);

    //            break;

    //         case Navigate.previous :

    //            _currentPageIndex--;
    //            GetChapter(_currentPageIndex);

    //            break;
    //    }

    //    previousBtn.interactable = _currentPageIndex != 0;

    //    // header.text = "Chapter " + pagePanels.Find(x => x.gameObject.activeInHierarchy).Page.chapterIndex;
    //}
}

[Serializable]
public class Chapter
{
    public int chapterIndex;
    public int pageCount;

    public string chapterID;
    public string chapterName;
    public string chapterDescription;

    public Chapter (int chapterIndex, string chapterID, string chapterName, string chapterDescription)
    {
        this.chapterIndex = chapterIndex;
        this.chapterID = chapterID;
        this.chapterName = chapterName;
        this.chapterDescription = chapterDescription;
    }
}

[Serializable]
public class Question
{
    public string id;
    public string title;
   
    public List<Answer> answers;
    
    public Question(string id, string title, List<Answer> answers)
    {
        this.id = id;
        this.title = title;
        this.answers = answers;
    }
}

[Serializable]
public class Answer
{
    public string title;
    public bool option_status;
  
    public Answer(string title, bool optionStatus)
    {
        this.title = title;
        option_status = optionStatus;
    }
}
