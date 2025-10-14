using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using BibleGame.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

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

    [Header("Speaker")]
    [SerializeField] TextSpeech speech;

  

    /// <summary>
    /// Action imeplemented on enable
    /// </summary>
    private void OnEnable()
    {
       // previousBtn.onClick.AddListener(() =>  (Navigate.previous));
        nextBtn.onClick.AddListener(()=> 
        {
            speech.Stop();
            callback.EndBookAction();
            AudioManager.Instance.PlayButton();
        });


        backBtn.onClick.AddListener(() =>
        {
            speech.Stop();
            Actions.ChangePanelActions(CanvasType.home);
            AudioManager.Instance.PlayButton();
        });

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
            if (!success)
            {
                PopUp.Instance.EnableLoad(false);
                Debug.LogError("Error in getting chapter details");
                return;
            }

            mScorll.verticalNormalizedPosition = 1.0f;

            header.text =  $"Chapter {res.ResponseData.data.number}";

            Debug.Log($"<color=green> Chapter {res.ResponseData.data.id} is loaded.</color>");

            string content = res.ResponseData.data.content.ToString();

            Debug.Log($"Content :{content}");

            content = Utils.ConvertHtmlToPlainText(content);

            Debug.Log($"Cleaned Content :{content}");


            pagePanel.GetComponent<TranslateLang>().UpdateText();

            LanguageController.Instance.Translate(content, translation =>
            {
                pagePanel.text = translation;
              //  pagePanel.GetComponent<TranslateLang>().UpdateText();

                PopUp.Instance.EnableLoad(false);

                string[] versesArray = content.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
                List<string> versesList = new List<string>(versesArray);

                 speech.Initialise(translation, true);
            });

           

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

    public void SetStatus(bool optionStatus)
    {
        option_status = optionStatus;
    }
}
