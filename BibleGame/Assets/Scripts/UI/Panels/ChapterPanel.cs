using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using BibleGame;
using UnityEngine.SceneManagement;
using TMPro;

public class ChapterPanel : MonoBehaviour, ICover, IBookChapter, IBook, ISelectGame
{
    [Header("UI Settings:")]
    [SerializeField] Button settingsBtn;

    [Space]
    [SerializeField] private Cover coverPanel;
    [SerializeField] private Books books;
    [SerializeField] private SelectGame selectGame;
    [SerializeField] private GameObject chapterPanel;
    [SerializeField] private GameObject booksPanel;
    [SerializeField] private MCQManager mCQManager;
    [SerializeField] private AnagramManager anagramManager;

    StyleUI styleUI;

    /// <summary>
    /// Action implemented on enable
    /// </summary>
    private void OnEnable()
    {
        coverPanel.caklback = this;
        books.bookCallback = this;
        selectGame.CallbackSelectGame = this;
        chapterPanel.GetComponent<BookChapter>().callback = this;

        settingsBtn?.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.setPanel));

        coverPanel.gameObject.SetActive(true);
        chapterPanel.SetActive(false);
        mCQManager.gameObject.SetActive(false);
        anagramManager.gameObject.SetActive(false);
        booksPanel.SetActive(false);
        selectGame.gameObject.SetActive(false);
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        settingsBtn?.onClick.RemoveAllListeners();
    }

   
    public void SelectChapter(string chapter)
    {
        Debug.Log("Chapter !!!");

        coverPanel.gameObject.SetActive(false);
        chapterPanel.SetActive(false);
        selectGame.gameObject.SetActive(false);
        booksPanel.SetActive(true);
    }

    public void EndBookAction()
    {
        booksPanel.SetActive(false);
        chapterPanel.SetActive(false);
        mCQManager.gameObject.SetActive(false);
        anagramManager.gameObject.SetActive(false);
        selectGame.gameObject.SetActive(true);
    }

    public void BackToCover()
    {
        coverPanel.gameObject.SetActive(true);
        chapterPanel.SetActive(false);
        booksPanel.SetActive(false);
        selectGame.gameObject.SetActive(false);
    }

    public void BackToBooks()
    {
        coverPanel.gameObject.SetActive(false);
        chapterPanel.SetActive(false);
        selectGame.gameObject.SetActive(false);
        booksPanel.SetActive(true);
    }

    public void SelectBook(string bookName)
    {
        coverPanel.gameObject.SetActive(false);
        chapterPanel.SetActive(true);
        booksPanel.SetActive(false);
        selectGame.gameObject.SetActive(false);
    }

    public void PlayTriviaGame()
    {
        selectGame.gameObject.SetActive(false);
        mCQManager.gameObject.SetActive(true);
        anagramManager.gameObject.SetActive(false);
    }

    public void PlayWordGame()
    {
        // SceneManager.LoadScene(1);
        selectGame.gameObject.SetActive(false);
        mCQManager.gameObject.SetActive(false);
        anagramManager.gameObject.SetActive(true);
    }
}
