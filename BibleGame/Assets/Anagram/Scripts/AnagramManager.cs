using System;
using System.Collections;
using System.Collections.Generic;
using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnagramManager : MonoBehaviour, ICorrectPanel,IAnagramControl
{
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button hintBtn;
    [SerializeField] private Button submitBtn;
    [SerializeField] private Button hintSubmitBtn;
    [SerializeField] private Button hintCloseBtn;
    [SerializeField] private Button rewardBtn;
    [SerializeField] GameObject hintPanel;

    [SerializeField] TMP_Text coinsText;

    [Header("GameController:")]
    [SerializeField] GameController_Anagram gameController;

    int hintIndex = 0;

    private void OnEnable()
    {
       // Screen.orientation = ScreenOrientation.LandscapeLeft;

        hintBtn.onClick.AddListener(UseHint);
        hintPanel.SetActive(false);
        homeBtn.onClick.AddListener(() => 
        {
            AudioManager.Instance.PlayButton();
            Actions.ChangePanelActions(CanvasType.home);
        });
        rewardBtn.onClick.AddListener(() => 
        {
            AudioManager.Instance.PlayButton();
            Actions.ChangePanelActions(CanvasType.reward); 
        });

       submitBtn.onClick.AddListener(() => { SubmitAction(); AudioManager.Instance.PlayButton(); });

        submitBtn.interactable = false;
        submitBtn.onClick.AddListener(() => { NextAction(); AudioManager.Instance.PlayButton(); });
        
        hintBtn.onClick.AddListener(()=> 
        {
            ShowHint();
            AudioManager.Instance.PlayButton();
        });

        hintSubmitBtn.onClick.AddListener(()=>
        {
            UseHint();
            AudioManager.Instance.PlayButton();

        });

        hintCloseBtn.onClick.AddListener(() =>
        {
            CloseHint();
            AudioManager.Instance.PlayButton();
        });

        gameController.callback = this;

        UpdateCoins(UserData.coins);

       // hintBtn.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
       // Screen.orientation = ScreenOrientation.Portrait;

        homeBtn.onClick.RemoveAllListeners();
        hintBtn.onClick.RemoveAllListeners();
        submitBtn.onClick.RemoveAllListeners();
    }

    void Intialise()
    {
       
    }


    private void OnClickHomeBtn()
    {
        SceneManager.LoadScene(0);
    }

    private void OnClickHintBtn()
    {
        hintPanel.SetActive(true);
    }

    private void CloseHint()
    {
        hintPanel.SetActive(false);
    }
    private void ShowHint()
    {
       
    }

    private void UseHint()
    {
        if (gameController.CurrentQIndex > 5)
            return;

        int index = gameController.CurrentQIndex - 1;

        if (index < 0)
            index = 0;

        int coins = UserData.coins;
        coins -= 7;


        if (coins < 0)
        {
            coins = 0;
            UserData.coins = coins;
            UpdateCoins(coins);

            PopUp.Instance.ShowMessage($"Not enough coins !!");
            return;
        }

        UserData.coins = coins;
        UpdateCoins(coins);

        string correctAnswer = gameController.Questions[index].hint;

        string position = hintIndex switch
        {
            0 => "first",
            1 => "second",
            2 => "third",
            3 => "fourth",
            4 => "fifth",
            5 => "Sixth",
            6 => "Seventh",
            7 => "Eighth",
            _ => throw new NotImplementedException()
        };

        string hintData = $"{position} letter is {correctAnswer[hintIndex]}";

        hintIndex++;
        PopUp.Instance.ShowMessage($"Hint:{hintData}");
    }

    public void UpdateScore()
    {
        int coins = UserData.coins;

        coins += 4;
        UserData.coins = coins;

        UpdateCoins(coins);
    }

    public void UpdateCoins(int coins)
    {
        coinsText.text = coins.ToString(); 
    }
   
    public void RestartAction()
    {
        
    }

    public void NextAction()
    {
       // Actions.ChangePanelActions(CanvasType.home);
    }

    public void ActivateSubmitBtn(bool enable)
    {
        submitBtn.interactable = enable;
    }

    public void SubmitAction()
    {
        Debug.Log("Submitting....");
        UserData.coins = 20;
        UpdateCoins(UserData.coins);

        SubmitRequestData requestData = new SubmitRequestData()
        {
            level_id = GameData.levelID,
            coins = 20,
            ratings = 3,
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
            Actions.ChangePanelActions(CanvasType.home);

        }, requestData);
    }
}
