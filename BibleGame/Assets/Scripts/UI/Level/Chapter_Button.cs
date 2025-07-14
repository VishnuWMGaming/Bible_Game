using System.Collections;
using System.Collections.Generic;
using BibleGame.API;
using UnityEngine;

using UnityEngine.UI;

using DG.Tweening;

public interface IChapterButton
{
    public void OnLevelSelectAction();
}

[RequireComponent(typeof(Button))]
public class Chapter_Button : MonoBehaviour
{
    Button button;

    [Header("UI Settings")]
    [SerializeField] List<GameObject> stars = new List<GameObject>();

    [Tooltip("Enter the level value which it represents")]
    [SerializeField] int _levelIndex = 0;

    [SerializeField] private string chapterId = "";

    public int Level
    {
        get
        {
            return _levelIndex;
        }
        set
        {
            _levelIndex = value;
        }
    }

    public string ChapterID
    {
        get
        {
            return chapterId;
        }
        set
        {
            chapterId = value;
        }
    }
    public IChapterButton callback;

    /// <summary>
    /// Action  implemented on enable
    /// </summary>
    private void OnEnable()
    {
        if (callback == null) 
        button = GetComponent<Button>();

        button.onClick.AddListener(() => LevelSelect(_levelIndex));
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        if (button != null) 
            button.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Action implemented on level select
    /// </summary>
    /// <param name="level"></param>
    void LevelSelect(int level)
    {
        Display(true);
        
        GameData.SetCurrentChapterID(chapterId);
        GameData.SetCurrentChapterIndex(_levelIndex);
        callback.OnLevelSelectAction();
        
        GetQuestions();
    }

    private void GetQuestions()
    {
       // GetQuestionsRequestData getQuestionsRequestData = new GetQuestionsRequestData(GameData.GetCurrentChapterID());
       // GetQuestionsAPI.GetQuestions(getQuestionsRequestData, GetQuestionsCallback);
    }

    //private void GetQuestionsCallback(bool success, GetQuestionsResponse response)
    //{
    //    if (success)
    //    {
    //        List<Question> tempQuestions = new List<Question>();
    //        foreach (var question in response.ResponseData)
    //        {
    //            List<Answer> tempAnswers = new List<Answer>();
    //            foreach (var answer in question.answers)
    //            {
    //                Answer temAnswer = new Answer(answer.title, answer.option_status);
    //                tempAnswers.Add(temAnswer);
    //            }

    //            Question tempQues = new Question(question.id, question.title, tempAnswers);
    //            tempQuestions.Add(tempQues);
    //        }
            
    //        GameData.SetQuestions(tempQuestions);
    //    }
    //}

    void DisplayStars(int value)
    {
        if(value >3 || value <=0)
            return;

        for(int i=0 ; i<stars.Count; i++)
            stars[i].SetActive(false);

        for(int i = 0; i< value; ++i)
            stars[i].SetActive(true);
    }

    /// <summary>
    /// Display animation of the button
    /// </summary>
    /// <param name="isSelect"></param>
    public void Display(bool isSelect)
    {
        if (isSelect)
            button.gameObject.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 1.0f);
        else
            button.gameObject.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 1.0f);
    }
}
