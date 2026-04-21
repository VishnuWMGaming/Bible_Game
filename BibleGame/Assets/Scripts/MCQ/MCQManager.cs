using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using DebugUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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

    int hintIndex = 0;

    int wrongAnswer;

    TranslateLang qTrans;
    List<string> usedWrongs = new List<string>();

    GetQuestionsRequestData mCurrentrequestData;

    private void Awake()
    {
        qTrans = questionTxt.GetComponent<TranslateLang>();
    }

    /// <summary>
    /// Action implemented one enable
    /// </summary>
    private void OnEnable()
    {
        UserData.coins = AppData.coins;

        questionPanel.gameObject.SetActive(true);

        optionPanel.callback = this;
        correctPanel.callback = this;

        _homeBtn.onClick.AddListener(() => 
        {
            AudioManager.Instance.PlayButton();
            Actions.StartPageAction(StartPage.game_menu);
        });

        _submitBtn.onClick.AddListener(() => 
        {
            AudioManager.Instance.PlayButton();
            optionPanel.CheckAnswerAction(); 
        });

        wrongAnswer = 0;

        rewardBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.reward); });

        scoreText.text = UserData.coins.ToString();

        _submitBtn.interactable = false;
        
        hintBtn.onClick.AddListener(() => { ShowHint(); AudioManager.Instance.PlayButton(); });
       // hintSubmitBtn.onClick.AddListener(() => { UseHint(); AudioManager.Instance.PlayButton(); });
        hintCloseBtn.onClick.AddListener(() => { CloseHint(); AudioManager.Instance.PlayButton(); });

        //RestartAction();
        currentQuestionIndex = 0;

        Initialise();

        hintIndex = 0;
        //SetData(currentQuestionIndex);
    }

    /// <summary>
    /// Action imeplemented om disable
    /// </summary>
    private void OnDisable()
    {
        _homeBtn.onClick.RemoveAllListeners();
        _submitBtn.onClick.RemoveAllListeners();
        hintBtn.onClick.RemoveAllListeners();
      //  hintSubmitBtn.onClick.RemoveListener(UseHint);
        hintCloseBtn.onClick.RemoveListener(CloseHint);

        correctPanel.gameObject.SetActive(false);
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

        mCurrentrequestData = new GetQuestionsRequestData()
        {
            game_id = UserData.gameid,
            ageGroup = ageVal.ToString(),
            language = "en",
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

            foreach (var data in res.ResponseData.resArr)
            {
                List<Answer> answers = new List<Answer>();

                answers.Add(new Answer(data.opt1, false));
                answers.Add(new Answer(data.opt2, false));
                answers.Add(new Answer(data.opt3, false));
                answers.Add(new Answer(data.opt4, false));

                string answer = data.hint.Replace("Correct answer: ", "");

                int ansindex = -1;

                if (int.TryParse(answer, out ansindex))
                {
                    Debug.Log("Answer at: " + ansindex);

                    answers[ansindex - 1].SetStatus(true);
                   
                    Question question = new Question(data._id, data.title, answers);
                    questions.Add(question);
                }
                else
                {
                    Debug.LogError("Unable to answer string!");
                }
            }

            SetData(currentQuestionIndex);

        }, mCurrentrequestData);
    }

    public void EnableSubmit(bool enabled)
    {
        _submitBtn.interactable = enabled;
    }

    public void CorrectAnswerAction()
    {
        questionPanel.gameObject.SetActive(false);
        correctPanel.gameObject.SetActive(true);

        currentQuestionIndex++;

        AudioManager.Instance.PlaySFX(AudioType.Correct);
    }

    public void WrongAnswer()
    {
        AudioManager.Instance.PlaySFX(AudioType.Wrong);
        wrongAnswer++;

        Debug.LogError($"Wrong >>>>> {wrongAnswer}");

        if (wrongAnswer > 1)
        {
            Debug.LogWarning($"Reloading....{currentQuestionIndex}");
            ReloadQuestions(questions[currentQuestionIndex]?.title);
        }
    }

    void ReloadQuestions(string questionReload)
    {
        //int ageVal = UserData.currentAge switch
        //{
        //    AgeGroup.kindergarden => 1,
        //    AgeGroup.elementary => 2,
        //    AgeGroup.teenagers => 3,
        //    AgeGroup.adult => 4,
        //    _ => 1
        //};

        //GetQuestionsRequestData requestData = new GetQuestionsRequestData()
        //{
        //    game_id = UserData.gameid,
        //    ageGroup = ageVal.ToString(),
        //    language = "en",
        //    game_type = "objective",
        //    bible_id = UserData.bibleId,
        //    chapter_id = UserData.chapterId,
        //    book_id = UserData.bookId,
        //};

        wrongAnswer = 0;
        questions.Clear();

        PopUp.Instance.EnableLoad(true);

       // currentQuestionIndex = 0;

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

            foreach (var data in res.ResponseData.resArr)
            {
                List<Answer> answers = new List<Answer>();

                answers.Add(new Answer(data.opt1, false));
                answers.Add(new Answer(data.opt2, false));
                answers.Add(new Answer(data.opt3, false));
                answers.Add(new Answer(data.opt4, false));

                string answer = data.hint.Replace("Correct answer: ", "");

                int ansindex = -1;

                if (int.TryParse(answer, out ansindex))
                {
                    Debug.Log("Answer at: " + ansindex);

                    answers[ansindex - 1].SetStatus(true);

                    Question question = new Question(data._id, data.title, answers);
                    questions.Add(question);
                }
                else
                {
                    Debug.LogError("Unable to answer string!");
                }
            }

            if(questionReload == questions[currentQuestionIndex]?.title)
            {
                ReloadQuestions(questionReload);
                return;
            }


            SetData(currentQuestionIndex);

        }, mCurrentrequestData);
    }

    public void RestartAction()
    {
        questionPanel.gameObject.SetActive(true);
        correctPanel.gameObject.SetActive(false);

        _submitBtn.interactable = false;
    }

    public void NextAction()
    {
        UserData.coins += 4;

        UpdateCoins(UserData.coins);

        wrongAnswer = 0;


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

            SubmitAction(() =>
            {
                PopUp.Instance.ShowMessage($"You have completed the chapter {UserData.chapterId}", null, MScreenOriatation.portrait);
                AudioManager.Instance.PlaySFX(AudioType.Win);

                Actions.ChangePanelActions(CanvasType.home);
            });
        }
    }


    public void SubmitAction(Action onComplete)
    {
        if (String.IsNullOrEmpty(GameData.levelID))
        {
            Debug.LogError("level id is not initialised");
            return;
        }

        int coins = AppData.coins > 0 ? UserData.coins - AppData.coins : UserData.coins;

        Debug.Log($"Submitting.... {AppData.coins} - {UserData.coins} = {coins} ");
        //  UserData.coins = 20;

        scoreText.text = UserData.coins.ToString();

        if (coins ==0)
            coins = 0;

        SubmitRequestData requestData = new SubmitRequestData()
        {
            level_id = GameData.levelID,
            coins = coins,
            ratings = 3,
            queAttempCount = currentQuestionIndex,
            question_data = null
        };

        PopUp.Instance.EnableLoad(true);
        GetQuestionsAPI.SubmitAnswer((success, res) =>
        {
            PopUp.Instance.EnableLoad(false);
            if (!success)
            {
                Debug.LogError("Error in submitting the answer");
                return;
            }

            Debug.Log("<color=green>Submitted </color>");
            onComplete?.Invoke();

            //Actions.ChangePanelActions(CanvasType.home);

        }, requestData);
    }

    private void ShowHint()
    {
        HintAPI.GetFreeHint((success,res) =>
        {
            if(!success)
            {
                PopUp.Instance.ShowMessage("Unable to get the free hints");
                return;
            }

            int freeHints = res.ResponseData.freeHint;

            if (freeHints <= 0)
            {
                if(UserData.coins < 7)
                {
                    PopUp.Instance.ShowMessage($"Not enough coins !!");
                    return;
                }

                int coins = UserData.coins;
                coins -= 7;

                if (coins < 0)
                coins = 0;

                UpdateCoins(coins);

                HintAction();
                return;
            }

            HintAction();

            HintAPI.DeductFreeHint((success) =>
            {
                if(!success)
                {
                    //  PopUp.Instance.ShowMessage("Unable to get the free hints");
                }
            });

            return;
        });
    }

    private void HintAction()
    {
        if (currentQuestionIndex > questions.Count)
            return;

        int index = currentQuestionIndex;

        if (index < 0)
            index = 0;

        if(hintIndex > 1)
        {
            PopUp.Instance.ShowMessage("Could not use more hints !!");
            return;
        }

        string wrongAnswer = questions[index].answers.Where(x => !x.option_status  && !usedWrongs.Contains(x.title) )
                      .OrderBy(x => UnityEngine.Random.value)
                      .FirstOrDefault()?.title;

        //string mhintAnswer = $"Hint : {wrongAnswer}";

        DevDebug.Log($"Wrong Answer : {wrongAnswer}",DebugColor.Indigo);

        optionPanel.EnableInteractable(wrongAnswer, false);

        hintIndex++;

        usedWrongs.Add(wrongAnswer);

        //LanguageController.Instance.Translate(mhintAnswer, (translation) =>
        //{
        //    PopUp.Instance.ShowMessage(translation);
        //});
    }

    void UpdateCoins(int coins)
    {
        UserData.coins = coins;
        scoreText.text = UserData.coins.ToString();
    }

    private void CloseHint()
    {
        hintPanel.SetActive(false);
    }


    private void SetData(int questionIndex)
    {
        hintIndex = 0;
        usedWrongs.Clear();


        DebugUtils.DevDebug.Log($"Initialise question : {questions[questionIndex]?.title} :: {questionIndex}",DebugColor.Turquoise);
        questionTxt.text = "";
        questionTxt.text = questions[questionIndex]?.title;

        qTrans.UpdateValue(questions[questionIndex]?.title);

        qTrans.UpdateText(() =>
        {
            //textSpeech.Initialise(questionTxt.text, true);

           // AudioManager.Instance.PlayVoice(questionTxt.text);
        });

        optionPanel.SetOptions(questions[questionIndex]?.answers);

        UpdateCoins(UserData.coins);
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
