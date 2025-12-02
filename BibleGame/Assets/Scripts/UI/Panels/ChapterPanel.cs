using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using BibleGame;
using UnityEngine.SceneManagement;
using TMPro;
using BibleGame.Data;

public class ChapterPanel : MonoBehaviour, ICover, IBookChapter, IBook, ISelectGame
{
    [Header("UI Settings:")]
    [SerializeField] Button settingsBtn;
    [SerializeField] Button homeBtn;

    [SerializeField] private Books books;
    [SerializeField] private Chapters chapters;
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
        //coverPanel.caklback = this;

        //if (AppData.mCurrentPage != null)
        //    SelectPage(AppData.mCurrentPage);

        Actions.StartPageAction += SelectPage;

        books.bookCallback = this;
        selectGame.CallbackSelectGame = this;
        chapterPanel.GetComponent<BookChapter>().callback = this;

        settingsBtn?.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.setPanel); });
        homeBtn?.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.home); });
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        Actions.StartPageAction -= SelectPage;
        settingsBtn?.onClick.RemoveAllListeners();
        homeBtn?.onClick.RemoveAllListeners();

        booksPanel.SetActive(false);
        chapterPanel.SetActive(false);
        mCQManager.gameObject.SetActive(false);
        anagramManager.gameObject.SetActive(false);
        selectGame.gameObject.SetActive(false);
        chapters.gameObject.SetActive(false);
    }


    void Intialise()
    {

    }

    public void SelectPage(StartPage page)
    {
        booksPanel.SetActive(page == StartPage.book);
        chapterPanel.SetActive(page == StartPage.chapter);
        mCQManager.gameObject.SetActive(page == StartPage.game_MCQ);
        anagramManager.gameObject.SetActive(page == StartPage.game_anagram);
        selectGame.gameObject.SetActive(page == StartPage.game_menu);
        chapters.gameObject.SetActive(page == StartPage.selectChapter);
    }

   
    public void SelectChapter(string chapter)
    {
        //coverPanel.gameObject.SetActive(false);
        chapterPanel.SetActive(false);
        selectGame.gameObject.SetActive(false);
        booksPanel.SetActive(true);
    }

    public void EndBookAction()
    {
        Debug.Log("Chapters finished !!!!!!!!!!!!!!!!!!");

        SelectPage(StartPage.game_menu);
    }

    public void BackToCover()
    {
        //coverPanel.gameObject.SetActive(true);
        chapterPanel.SetActive(false);
        booksPanel.SetActive(false);
        selectGame.gameObject.SetActive(false);
    }

    public void BackToBooks()
    {
        //coverPanel.gameObject.SetActive(false);
        chapterPanel.SetActive(false);
        selectGame.gameObject.SetActive(false);
        booksPanel.SetActive(true);
    }

    public void SelectBook(string bookName)
    {
       // coverPanel.gameObject.SetActive(false);
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

public enum StartPage { book,selectChapter,chapter,game_MCQ,game_anagram ,game_menu};