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

    enum Navigate { previous , next };


    public IBookChapter callback;

    int _currentPageIndex = 0;

    /// <summary>
    /// Action imeplemented on enable
    /// </summary>
    private void OnEnable()
    {
        previousBtn.onClick.AddListener(() => PageNavigateAction(Navigate.previous));
        nextBtn.onClick.AddListener(()=>PageNavigateAction(Navigate.next));
        backBtn.onClick.AddListener(() => callback.BackToCover());

        _currentPageIndex = 0;  

        for (int i = 0; i < pagePanels.Count; ++i)
            pagePanels[i].gameObject.SetActive(false);

        pagePanels[0].gameObject.SetActive(true);
        header.text = "Chapter " + pagePanels.Find(x => x.gameObject.activeInHierarchy).Page.chapterIndex;

        previousBtn.interactable =false;
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

        header.text = "Chapter " + pagePanels.Find(x => x.gameObject.activeInHierarchy).Page.chapterIndex;
    }
}

[Serializable]
public class Chapter
{
    public int chapterIndex;
    public int pageCount;

    public Chapter (int chapterIndex, int pageCount)
    {
        this.chapterIndex = chapterIndex;
        this.pageCount = pageCount;
    }
}
