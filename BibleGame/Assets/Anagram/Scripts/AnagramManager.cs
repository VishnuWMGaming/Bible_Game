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

    private void OnEnable()
    {
        hintBtn.onClick.AddListener(UseHint);
        hintPanel.SetActive(false);
        homeBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.home));
        rewardBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.reward));
       submitBtn.onClick.AddListener(SubmitAction);

        submitBtn.interactable = false;
        submitBtn.onClick.AddListener(NextAction);
        
        hintBtn.onClick.AddListener(ShowHint);
        hintSubmitBtn.onClick.AddListener(UseHint);
        hintCloseBtn.onClick.AddListener(CloseHint);

        gameController.callback = this;

        UpdateCoins(UserData.coins);
    }

    private void OnDisable()
    {
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

        PopUp.Instance.ShowMessage($"Hint : {correctAnswer}");
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
