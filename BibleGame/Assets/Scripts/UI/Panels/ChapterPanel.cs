using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using BibleGame;

public class ChapterPanel : MonoBehaviour,ICover, IBookChapter
{
    [Header("UI Settings:")]
    [SerializeField] Button settingsBtn;

    [Space]
    [SerializeField] Cover coverPanel;
    [SerializeField] GameObject chapterPanel;
    [SerializeField] MCQManager mCQManager;

    /// <summary>
    /// Action implemented on enable
    /// </summary>
    private void OnEnable()
    {
        coverPanel.caklback = this;
        chapterPanel.GetComponent<BookChapter>().callback = this;

        settingsBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.setPanel));

        coverPanel.gameObject.SetActive(true);
        chapterPanel.SetActive(false);
        mCQManager.gameObject.SetActive(false);
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        settingsBtn.onClick.RemoveAllListeners();
    }

    public void SelectChapter(string chapter)
    {
        Debug.Log("Chapter !!!");

        coverPanel.gameObject.SetActive(false);
        chapterPanel.SetActive(true);
    }

    public void EndBookAction()
    {
        chapterPanel.SetActive(false);
        mCQManager.gameObject.SetActive(true);
    }

    public void BackToCover()
    {
        coverPanel.gameObject.SetActive(true);
        chapterPanel.SetActive(false);
    }
}
