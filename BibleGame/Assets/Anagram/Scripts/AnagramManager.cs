using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Tilemaps.Tilemap;

public class AnagramManager : MonoBehaviour, ICorrectPanel, IAnagramControl
{
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button hintBtn;
    [SerializeField] private Button hintSubmitBtn;
    [SerializeField] private Button hintCloseBtn;
    [SerializeField] private Button rewardBtn;
    [SerializeField] GameObject hintPanel;

    [SerializeField] TMP_Text coinsText;

    [Header("GameController:")]
    [SerializeField] GameController_Anagram gameController_Landscape;
    [SerializeField] GameController_Anagram gameController_Portrait;

    GameController_Anagram gameController;

    [Header("Orientation")]
    [SerializeField] MScreenOriatation screenOriatation;
    [SerializeField] Button mScreenOrientButton;


    int hintIndex = 0;

    private void OnEnable()
    {
        #region ORIENTATION
        PopUp.Instance.EnableLoad(true);

        AppData.orientation = screenOriatation;

       // screenOriatation = MScreenOriatation.landscape;
        mScreenOrientButton?.onClick.AddListener(() =>
        {
            screenOriatation = screenOriatation == MScreenOriatation.landscape ? MScreenOriatation.portrait : MScreenOriatation.landscape;
            Orientation();
        });

        Orientation();

        PopUp.Instance.EnableLoad(false);
        #endregion

        hintBtn.onClick.AddListener(UseHint);
        hintPanel.SetActive(false);
        homeBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayButton();
            Actions.StartPageAction(StartPage.game_menu);
        });

        rewardBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayButton();
            Actions.ChangePanelActions(CanvasType.reward);
        });

        hintBtn.onClick.AddListener(() =>
        {
            ShowHint();
            AudioManager.Instance.PlayButton();
        });

        hintSubmitBtn.onClick.AddListener(() =>
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

       // AppData.orientation = MScreenOriatation.landscape;
    }

    private void OnDisable()
    {
        homeBtn.onClick.RemoveAllListeners();
        hintBtn.onClick.RemoveAllListeners();
        mScreenOrientButton?.onClick.RemoveAllListeners();
    }

    void Orientation()
    {
        if(gameController != null)
        gameController.gameObject.SetActive(false);

        gameController = screenOriatation switch
        {
            MScreenOriatation.portrait => gameController_Portrait,
            MScreenOriatation.landscape => gameController_Landscape,
        };

        gameController.gameObject.SetActive(true);
        gameController.callback = this;
    }

    public void NextQAction()
    {

    }

    private void CloseHint()
    {
        hintPanel.SetActive(false);
    }
    private void ShowHint()
    {
        hintIndex = 0;
    }

    private void UseHint()
    {
        HintAPI.GetFreeHint((success, res) =>
        {
            if (!success)
            {
                PopUp.Instance.ShowMessage("Unable to get the free hints");
                return;
            }

            int freeHints = res.ResponseData.freeHint;

            if (freeHints <= 0)
            {
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

                HintAction();
                return;
            }


            HintAction();

            HintAPI.DeductFreeHint((success) =>
            {
                if (!success)
                {
                    //  PopUp.Instance.ShowMessage("Unable to get the free hints");
                }
            });

            return;
        });
    }

    public void HintAction()
    {
        if (gameController.CurrentQIndex > 5)
            return;

        int index = gameController.CurrentQIndex - 1;

        if (index < 0)
            index = 0;

        string correctAnswer = gameController.Questions[index].hint;

        if (hintIndex > 7)
            hintIndex = 0;

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

        string hintMessage = $"Hint:{hintData}";

        LanguageController.Instance.Translate(hintMessage, (translation) =>
        {
            PopUp.Instance.ShowMessage(translation);
        });
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
        UserData.coins = coins;
    }

    public void RestartAction()
    {
        hintIndex = 0;
    }

    public void NextAction()
    {
        // Actions.ChangePanelActions(CanvasType.home);
    }

    public void ActivateSubmitBtn(bool enable)
    {
       
    }

    public void SubmitAction()
    {
        Debug.Log("Submitting....");

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

public enum MScreenOriatation { portrait, landscape }
