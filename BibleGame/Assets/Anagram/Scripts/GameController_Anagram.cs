using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using DebugUtils;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public interface IAnagramControl
{
    public void ActivateSubmitBtn(bool enable);
    public void SubmitAction();
    public void UpdateScore();

    public void NextQAction();
}

public class GameController_Anagram : MonoBehaviour,IBox
{
    [SerializeField] private RectTransform lettersParent;
    HorizontalLayoutGroup layoutGroup;

    [SerializeField] Button mSubmitButton;
    [SerializeField] Button mHintButton;
    [SerializeField] Button mBackButton;

    [Header("Boxes:")]
    [SerializeField] List<GramBox> gramBoxes = new List<GramBox>();

    [SerializeField] public List<AnagramLetter> _anagramLetters = new List<AnagramLetter>();

    string result = string.Empty;

    [SerializeField] GameObject congratsTxt;

    [Header("letterCount")]
    [SerializeField] int letterCount;

    [Header("Current Word Index"), SerializeField]
    private int currentWordIndex = 0;

    [Header("SpriteData")]
    [SerializeField] SpriteData spriteData;

    [Space]
    [SerializeField] List<GetQuestionsAnagramData> questions = new List<GetQuestionsAnagramData>();
    public List<GetQuestionsAnagramData> Questions => questions;

    StyleUI styleUI;

    [SerializeField] TMP_Text mTitle;

    [SerializeField] TMP_Text coinText;

    public IAnagramControl callback;

    #region LOCAL_VARIABLES
    [SerializeField] int currentQIndex = 0;
    public int CurrentQIndex => currentQIndex;

    bool isWon = false;
    int hintIndex = 0;
    #endregion


    [Header("Screen Orintation:")]
    [SerializeField] ScreenOrient screenOrient;
    public ScreenOrient ScreenOrient => screenOrient;

    [Header("TextSpeech")]
    [SerializeField] TextSpeech speech;

    private void OnEnable()
    {
        layoutGroup = lettersParent.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.enabled = true;

        styleUI = spriteData.GetStyle(UserData.currentAge);

        foreach (var grambox in gramBoxes)
            grambox.SetImage(styleUI.box);

        mSubmitButton?.onClick.AddListener(() =>
        {
            if (callback != null)
                callback.SubmitAction();
        });
        mSubmitButton.interactable = false;

        mHintButton?.onClick.AddListener(() => 
        {
            
        });

        mBackButton?.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayButton();
            Actions.StartPageAction(StartPage.game_menu);

        });

        currentQIndex = 0;
        hintIndex = 0;
        SetData();
    }

    private void OnDisable()
    {
        mSubmitButton?.onClick.RemoveAllListeners();
        mHintButton?.onClick.RemoveAllListeners();
        mBackButton?.onClick.RemoveAllListeners();
    }

    void SetData()
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
        GetQuestionsAPI.GetQuestionsAnagram((success,res) =>
        {
            PopUp.Instance.EnableLoad(false);
            if (!success)
            {
                Debug.LogError("Error in getting question in anagram");
                Actions.StartPageAction(StartPage.game_menu);
                return;
            }

            GameData.levelID = res.ResponseData.levelData._id;

            questions = res.ResponseData.resArr;
            GameInitilise(questions[currentQIndex]);

            callback.NextQAction();
            currentQIndex++;

        }, requestData);

    }

    public void NextQAct()
    {
        if (!isWon)
            return;

        isWon = false;

        callback.UpdateScore();

        if (currentQIndex > questions.Count-1)
        {
            Debug.Log("Get read to submit !!!");
           // callback.ActivateSubmitBtn(true);
            mSubmitButton.interactable = true;
            return;
        }

        PopUp.Instance.EnableLoad(true);

        GameInitilise(questions[currentQIndex]);
        currentQIndex++;
    }

    void GameInitilise(GetQuestionsAnagramData question)
    {
        string word = question.hint;
        string[] letterVals = word.Select(c => c.ToString()).ToArray();

        mTitle.text = question.title;
        //mTitle.GetComponentInChildren<TranslateLang>().UpdateText(() =>
        //{

        //});

        speech.Initialise(question.title, true);

        _anagramLetters.Clear();

        if (letterVals.Length > 8)
        {
            Debug.LogError($"Exceeded the maximum grambox count: {question.hint}");
            return;
        }
        
        for (int i = 0;i< letterVals.Length;++i)
        {
            AnagramLetter letter = new AnagramLetter
            {
                index = i,
                val = letterVals[i]
            };

            _anagramLetters.Add(letter);
        }

        //Scrambing process...
        Debug.Log($"Scrambled word: {ShuffleWord(word)} ... Scrambling");

        foreach (var box in gramBoxes) box.Restart();
        layoutGroup.enabled = true;

        List<AnagramLetter> scrambledLetters = new List<AnagramLetter>();

        string scrambledWord = ShuffleWord(word);
        string[] scrambledletterVals = scrambledWord.Select(c => c.ToString()).ToArray();

        for (int i = 0; i < scrambledletterVals.Length; ++i)
        {
            AnagramLetter letter = new AnagramLetter
            {
                index = i,
                val = scrambledletterVals[i]
            };

            scrambledLetters.Add(letter);
        }

        //Scrambing process...
        Debug.Log($"Scrambling 1");

        for (int index = 0; index < scrambledLetters.Count; index++)
        {
            var letters = scrambledLetters[index];

            GramBox box = gramBoxes[index];
            box.Enable(true);

            box.SetLetter(letters.val);
            box.SetIndex(letters.index);

            //box.SetImage(styleUI.box);
            box.callback = this;
        }

        foreach (var box in gramBoxes) box.Enable(box.Index != -1);

        Debug.Log($"Scrambling 2");

        LayoutRebuilder.ForceRebuildLayoutImmediate(lettersParent);

        foreach (var box in gramBoxes) box.SetStartPos(box.BoxT.anchoredPosition, box.BoxT.position);
        layoutGroup.enabled = false;
        foreach (var box in gramBoxes) box.Arrange();


        Debug.Log("Anagram  is set ");
        PopUp.Instance.EnableLoad(false);

    }

    string ShuffleWord(string word)
    {
        System.Random rng = new System.Random();
        return new string(word.OrderBy(c => rng.Next()).ToArray());
    }

    public void UpdatedPos(int index, Vector2 position)
    {
        GramBox currentBox =  gramBoxes.Find(x => x.Index == index && x.IsEnable && !x.isCorrect);

        List<GramBox> otherBoxes = gramBoxes.Where(p => p != currentBox  && p.IsEnable && !p.IsDrag && !p.isCorrect).ToList();

        foreach (GramBox box in otherBoxes) box.m_IsAble = false;

        foreach (GramBox box in otherBoxes)
        {
            DevDebug.Log($"Checking box {currentBox.Value} with {box.Value}",DebugColor.Gold);

            if(AnagramUtils.AreImagesOverlapping(currentBox.BoxT, box.BoxT))
            {
                Vector2 newPos = box.BoxV;
                int  newIndex = box.Index;

                box.SetPos(currentBox.BoxV,currentBox.Index,true);
                currentBox.SetPos(newPos,newIndex);
                
                break;
            }
        }
    }

    public void ResultAction()
    {
        Debug.LogWarning("Result !!!!!!!!!!!!!!!");

        result = string.Empty;
        int index = 0;
        StartCoroutine(CheckingResult(index));
    }

    void EndGame()
    {
        isWon = false;
        StopAllCoroutines();

        Debug.Log("Checking the result..");

        List<GramBox> enabledBoxes = gramBoxes.Where(x => x.IsEnable).ToList();

        foreach (GramBox box in enabledBoxes) box.m_IsAble = true;

        for (int i = 0; i < enabledBoxes.Count; ++i)
        {
            isWon = enabledBoxes[i].isCorrect;

            if (!isWon)
                break;
        }

        if (isWon)
            Debug.LogWarning("Got the word");
        else
        {
            Debug.LogError("Word Not formed");
            return;
        }

        congratsTxt.SetActive(isWon);

       Invoke("NextQAct", 2.0f);
    }

    IEnumerator CheckingResult(int index)
    {
        EndGame();

        if (index >= letterCount)
        {
            StopAllCoroutines();
        }
        else
        {
            for (int i = 0; i < gramBoxes.Count; ++i)
            {
                if (index == gramBoxes[i].Index)
                {
                    result = result + gramBoxes[i].Value;
                    gramBoxes[i].SetCorrectWord(_anagramLetters.Find(x => x.index == index).val == gramBoxes[i].Value);
                   // Debug.Log("Checking .... " + index);
                    
                   // Debug.Log($"Checking 2 .... {_anagramLetters.Find(x => x.index == index).val}  {gramBoxes[i].Value}" );
                    
                    index++;
                    StartCoroutine(CheckingResult(index));
                }
            }
        }

        yield return null;
    }

    #region HINT

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

    private void UpdateCoins(int coins)
    {
       coinText.text = coins.ToString();
    }

    public void HintAction()
    {
        if (currentQIndex > 5)
            return;

        int index = currentQIndex - 1;

        if (index < 0)
            index = 0;

        string correctAnswer = questions[index].hint;

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
    #endregion

    void ClearAll()
    {
       
    }
}

public enum ScreenOrient {portrait,landscape}


[Serializable]
public class AnagramLetter
{
    public int index;
    public string val;
}

[Serializable]
public class AnagramLetterList
{
    public List<AnagramLetter> AnagramLetters = new List<AnagramLetter>();
}