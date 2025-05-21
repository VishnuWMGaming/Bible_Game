using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using System;


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

    private Chapter chapter;

    [SerializeField] private PagePanel pagePanel;
    [SerializeField] private Transform pageParent;

    enum Navigate { previous , next };


    public IBookChapter callback;

    int _currentPageIndex = 0;

    [Header("SpriteData")]
    [SerializeField] SpriteData spriteData;

    /// <summary>
    /// Action imeplemented on enable
    /// </summary>
    private void OnEnable()
    {
        previousBtn.onClick.AddListener(() => PageNavigateAction(Navigate.previous));
        nextBtn.onClick.AddListener(()=>PageNavigateAction(Navigate.next));
        backBtn.onClick.AddListener(() => callback.BackToBooks());

        _currentPageIndex = 0;  
        chapter = GameData.GetDataWithChapterID(GameData.GetCurrentChapterID());
        header.text = chapter.chapterName;

        StyleUI styleUI = spriteData.GetStyle(GameData.currentAge);

        if (pageParent.childCount > 4)
        {
            pagePanels.RemoveAt(0);
            Destroy(pageParent.GetChild(4).gameObject);
        }
        
        /*int pageCount = Mathf.CeilToInt(chapter.chapterDescription.Length / 100);
        pageCount++;
        for (int i = 0; i < pageCount; i++)
        {*/
            PagePanel temp = Instantiate(pagePanel, pageParent);
            pagePanels.Add(temp);
        /*}*/

        for (int i = 0; i < pagePanels.Count; i++)
        {
            pagePanels[i].GetComponent<TMP_Text>().text = chapter.chapterDescription;
            pagePanels[i].GetComponent<TMP_Text>().color = styleUI.textColor;
        }

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
        backBtn.onClick.RemoveAllListeners();   
    }


    void PageNavigateAction(Navigate navigate)
    {
        int index = pagePanels.IndexOf(pagePanels.Find(x => x.gameObject.activeInHierarchy));

        for (int i = 0; i < pagePanels.Count; ++i)
            pagePanels[i].gameObject.SetActive(false);

        switch (navigate)
        {
            case Navigate.next :

                if (index + 1 < pagePanels.Count)
                {
                    pagePanels[index + 1].gameObject.SetActive(true);
                    _currentPageIndex = index +1;
                }
                else
                {
                    callback.EndBookAction();
                    return;
                }

                break;

             case Navigate.previous :

                if (index - 1 >= 0)
                {
                    pagePanels[index - 1].gameObject.SetActive(true);
                    _currentPageIndex = index - 1;

                } // pagePanels[pagePanels.Count-1].gameObject.SetActive(true);

                break;
        }

        previousBtn.interactable = _currentPageIndex != 0;

        // header.text = "Chapter " + pagePanels.Find(x => x.gameObject.activeInHierarchy).Page.chapterIndex;
    }
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
