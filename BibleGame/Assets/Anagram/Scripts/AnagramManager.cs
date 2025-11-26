using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using DebugUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Tilemaps.Tilemap;

public class AnagramManager : MonoBehaviour, ICorrectPanel
{
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

    [Space]
    [SerializeField] List<GetQuestionsAnagramData> questions = new List<GetQuestionsAnagramData>();
    public List<GetQuestionsAnagramData> Questions => questions;



    int hintIndex = 0;

    private void OnEnable()
    {
        GameData.QIndex = 0;

        #region ORIENTATION
        PopUp.Instance.EnableLoad(true);

        Actions.ChangeLandscape += Orientation;

        mScreenOrientButton.interactable = false;

       // screenOriatation = MScreenOriatation.landscape;
       mScreenOrientButton?.onClick.AddListener(() =>
        {
            screenOriatation = screenOriatation == MScreenOriatation.landscape ? MScreenOriatation.portrait : MScreenOriatation.landscape;
            Orientation(screenOriatation);
        });

        SetData(() =>
        {
            Orientation(screenOriatation);
            PopUp.Instance.EnableLoad(false);

            mScreenOrientButton.interactable = true;
        });

        #endregion

    }

    private void OnDisable()
    {
        mScreenOrientButton?.onClick.RemoveAllListeners();

        Actions.ChangeLandscape -= Orientation;
    }

    void SetData(Action onComplete)
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
            language = LanguageController.Instance.GetLangStringVal(AppData.mLanguage),
            game_type = "anagram",
            bible_id = UserData.bibleId,
            chapter_id = UserData.chapterId,
            book_id = UserData.bookId,
        };

        PopUp.Instance.EnableLoad(true);
        GetQuestionsAPI.GetQuestionsAnagram((success, res) =>
        {
            PopUp.Instance.EnableLoad(false);
            if (!success)
            {
                Debug.LogError("Error in getting question in anagram");
                Actions.StartPageAction(StartPage.game_menu);
                return;
            }

            GameData.levelID = res.ResponseData.levelData._id;

            GameData.QIndex = 0;

            questions = res.ResponseData.resArr;

            GameData.anagramquestions = questions;

            onComplete?.Invoke();

            //GameInitilise(questions[currentQIndex]);

            //callback.NextQAction();
           // currentQIndex++;

        }, requestData);

    }

    public void Orientation(MScreenOriatation mscreenOrientation, bool isLock = false, bool isReload = true)
    {
        mScreenOrientButton.interactable = !isLock;

        if (isLock && screenOriatation == MScreenOriatation.landscape)
            return;

        screenOriatation = mscreenOrientation;
        AppData.orientation = screenOriatation;

        if(gameController != null)
          gameController.gameObject.SetActive(false);

         gameController = screenOriatation switch
         {
             MScreenOriatation.portrait => gameController_Portrait,
             MScreenOriatation.landscape => gameController_Landscape,
         };

           // gameController.isReload = isReload;
         gameController.gameObject.SetActive(true);
         gameController.SetData(questions);

        //gameController.callback = this;
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

    //private void UseHint()
    //{
    //    HintAPI.GetFreeHint((success, res) =>
    //    {
    //        if (!success)
    //        {
    //            PopUp.Instance.ShowMessage("Unable to get the free hints");
    //            return;
    //        }

    //        int freeHints = res.ResponseData.freeHint;

    //        if (freeHints <= 0)
    //        {
    //            int coins = UserData.coins;
    //            coins -= 7;


    //            if (coins < 0)
    //            {
    //                coins = 0;
    //                UserData.coins = coins;
    //                UpdateCoins(coins);

    //                PopUp.Instance.ShowMessage($"Not enough coins !!");
    //                return;
    //            }

    //            UserData.coins = coins;
    //            UpdateCoins(coins);

    //            HintAction();
    //            return;
    //        }


    //        HintAction();

    //        HintAPI.DeductFreeHint((success) =>
    //        {
    //            if (!success)
    //            {
    //                //  PopUp.Instance.ShowMessage("Unable to get the free hints");
    //            }
    //        });

    //        return;
    //    });
    //}

    //public void HintAction()
    //{
    //    if (gameController.CurrentQIndex > 5)
    //        return;

    //    int index = gameController.CurrentQIndex - 1;

    //    if (index < 0)
    //        index = 0;

    //    string correctAnswer = gameController.Questions[index].hint;

    //    if (hintIndex > 7)
    //        hintIndex = 0;

    //    string position = hintIndex switch
    //    {
    //        0 => "first",
    //        1 => "second",
    //        2 => "third",
    //        3 => "fourth",
    //        4 => "fifth",
    //        5 => "Sixth",
    //        6 => "Seventh",
    //        7 => "Eighth",
    //        _ => throw new NotImplementedException()
    //    };

    //    string hintData = $"{position} letter is {correctAnswer[hintIndex]}";

    //    hintIndex++;

    //    string hintMessage = $"Hint:{hintData}";

    //    LanguageController.Instance.Translate(hintMessage, (translation) =>
    //    {
    //        PopUp.Instance.ShowMessage(translation);
    //    });
    //}

    public void UpdateScore()
    {
        //int coins = UserData.coins;

        //coins += 4;
        //UserData.coins = coins;SW

       // UpdateCoins(coins);
    }

    //public void UpdateCoins(int coins)
    //{
    //    coinsText.text = coins.ToString();
    //    UserData.coins = coins;
    //}

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

}

public enum MScreenOriatation { portrait, landscape }
