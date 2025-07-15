using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using BibleGame;
using BibleGame.API;
using UnityEngine.Serialization;
using BibleGame.Data;
using System;

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
    [SerializeField] Button rewardBtn;

    [Space]
    [SerializeField] private List<Question> questions = new List<Question>();

   [SerializeField]  private int currentQuestionIndex = 0;
    
    /// <summary>
    /// Action implemented one enable
    /// </summary>
    private void OnEnable()
    {
        optionPanel.callback = this;
        correctPanel.callback = this;

        _homeBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.home));
        _submitBtn.onClick.AddListener(() => optionPanel.CheckAnswerAction());
        rewardBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.reward));

        scoreText.text = UserData.coins.ToString();

        _submitBtn.interactable = false;
        
        hintBtn.onClick.AddListener(ShowHint);
        hintSubmitBtn.onClick.AddListener(UseHint);
        hintCloseBtn.onClick.AddListener(CloseHint);

        //RestartAction();
        currentQuestionIndex = 0;

        Initialise();

        //SetData(currentQuestionIndex);
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

    void Initialise()
    {
        int ageVal = UserData.currentAge switch
        {
            AgeGroup.kindergarden => 1,
            AgeGroup.elementary => 2,
            AgeGroup.teenagers => 3,
            AgeGroup.adult => 4,
            _ => 1
        };

        GetQuestionsRequestData requestData = new GetQuestionsRequestData()
        {
            game_id = UserData.gameid,
            ageGroup = ageVal.ToString(),
            game_type = "objective",
            bible_id = UserData.bibleId,
            chapter_id = UserData.chapterId,
            book_id = UserData.bookId,
        };

        questions.Clear();

        PopUp.Instance.EnableLoad(true);

        currentQuestionIndex = 0;

        GetQuestionsAPI.GetQuestionsObjective((success, res) =>
        {
            PopUp.Instance.EnableLoad(false);

            if (!success)
            {
                Debug.LogError("Error in Get question objective");
                Actions.StartPageAction(StartPage.game_menu);
                return;
            }

            GameData.levelID = res.ResponseData.levelData._id;

            foreach (var data in res.ResponseData.questions)
            {
                List<Answer> answers = new List<Answer>();

                answers.Add(new Answer(data.opt1, false));
                answers.Add(new Answer(data.opt2, false));
                answers.Add(new Answer(data.opt3, false));
                answers.Add(new Answer(data.opt4, false));

                answers.Find(x => x.title == data.hint).option_status = true;

                Question question = new Question(data._id, data.title, answers);
                questions.Add(question);
            }

            SetData(currentQuestionIndex);

        }, requestData);
    }

    public void EnableSubmit(bool enabled)
    {
        _submitBtn.interactable = enabled;
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
        int coins = UserData.coins;
        coins += 4;
        UserData.coins = coins;

        scoreText.text = UserData.coins.ToString();

     //   Actions.UpdateCoins.Invoke(coins);

        if (currentQuestionIndex < questions.Count)
        {
            questionPanel.SetActive(true);
            correctPanel.gameObject.SetActive(false);
            _submitBtn.interactable = false;
            SetData(currentQuestionIndex);
        }
        else
        {
            if(String.IsNullOrEmpty(GameData.levelID))
            {
                Debug.LogError("level id is not initialised");
                return;
            }

            Debug.Log("Submitting....");
            UserData.coins = 20;

            scoreText.text = UserData.coins.ToString();

            SubmitRequestData requestData = new SubmitRequestData()
            {
                level_id = GameData.levelID,
                coins = 20,
                ratings = 3,
                question_data = null
            };

            PopUp.Instance.EnableLoad(true);
            GetQuestionsAPI.SubmitAnswer((success,res) =>
            {
                PopUp.Instance.EnableLoad(false);
                if (!success)
                {
                    Debug.LogError("Error in submitting the answer");
                    return;
                }

                Debug.Log("<color=green>Submitted </color>");
                Actions.ChangePanelActions(CanvasType.home);

            }, requestData);

        }
    }

    private void ShowHint()
    {
        if (currentQuestionIndex > questions.Count)
            return;

        int index = currentQuestionIndex - 1;

        if (index < 0)
            index = 0;

        int coins = UserData.coins;
        coins -= 7;

        if (coins < 0)
        {
            coins = 0;
            UserData.coins = coins;

            PopUp.Instance.ShowMessage($"Not enough coins !!");
            return;
        }
        UserData.coins = coins;

        scoreText.text = UserData.coins.ToString();

        string correctAnswer = questions[index].answers.Find(x => x.option_status).title;

        PopUp.Instance.ShowMessage($"Hint : {correctAnswer}");
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
        questionTxt.text = questions[questionIndex]?.title;

        optionPanel.SetOptions(questions[questionIndex]?.answers);
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
