using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using BibleGame;
using BibleGame.API;
using UnityEngine.Serialization;

public class MCQManager : MonoBehaviour,IOptionPanel,ICorrectPanel
{
    [Header("UI Settings:")]
    [SerializeField] OptionPanel optionPanel;
    [SerializeField] GameObject questionPanel;
    [SerializeField] GameObject hintPanel;
    [SerializeField] CorrectPanel correctPanel;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text questionTxt;
    [SerializeField] Button _homeBtn;
    [SerializeField] Button _submitBtn;
    [SerializeField] Button hintBtn;
    [SerializeField] Button hintSubmitBtn;
    [SerializeField] Button hintCloseBtn;
    private List<Question> questions = new ();

    private int currentQuestionIndex = 0;
    
    /// <summary>
    /// Action implemented one enable
    /// </summary>
    private void OnEnable()
    {
        optionPanel.callback = this;
        correctPanel.callback = this;

        _homeBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.home));
        _submitBtn.onClick.AddListener(() => optionPanel.CheckAnswerAction());

        _submitBtn.interactable = false;
        
        hintBtn.onClick.AddListener(ShowHint);
        hintSubmitBtn.onClick.AddListener(UseHint);
        hintCloseBtn.onClick.AddListener(CloseHint);

        RestartAction();
        currentQuestionIndex = 0;
        SetData(currentQuestionIndex);
    }

    public void EnableSubmit(bool enabled)
    {
        _submitBtn.interactable = enabled;
    }

    /// <summary>
    /// Action imeplemented om disable
    /// </summary>
    private void OnDisable()
    {
        _homeBtn.onClick.RemoveAllListeners();
        _submitBtn.onClick.RemoveAllListeners();
        hintBtn.onClick.RemoveListener(ShowHint);
        hintSubmitBtn.onClick.RemoveListener(UseHint);
        hintCloseBtn.onClick.RemoveListener(CloseHint);
    }

    public void CorrectAnswerAction()
    {
        questionPanel.gameObject.SetActive(false);
        correctPanel.gameObject.SetActive(true);
    }

    public void RestartAction()
    {
        questionPanel.gameObject.SetActive(true);
        correctPanel.gameObject.SetActive(false);

        _submitBtn.interactable = false;
    }

    public void NextAction()
    {
        if(currentQuestionIndex < questions.Count)
        {
            questionPanel.SetActive(true);
            correctPanel.gameObject.SetActive(false);
            _submitBtn.interactable = false;
            SetData(currentQuestionIndex);
        }
        else
        {
            Actions.ChangePanelActions(CanvasType.home);
        }
    }

    private void ShowHint()
    {
        hintPanel.SetActive(true);
    }

    private void UseHint()
    {
        
    }

    private void CloseHint()
    {
        hintPanel.SetActive(false);
    }


    private void SetData(int questionIndex)
    {
        questions = GameData.GetQuestions();

        questionTxt.text = questions[questionIndex].title;
        optionPanel.SetOptions(questions[questionIndex].answers);

        currentQuestionIndex++;
    }

    public int GetQuestionsCount()
    {
        return questions.Count;
    }

    public int GetCurrentIndex()
    {
        return currentQuestionIndex;
    }
    
}
