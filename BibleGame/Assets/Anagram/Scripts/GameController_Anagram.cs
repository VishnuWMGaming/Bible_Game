using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using UnityEngine.UIElements;
using System;
using System.Reflection;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using BibleGame.Data;
using BibleGame.API;

using System.Linq;
using TMPro;
using DebugUtils;


public interface IAnagramControl
{
    public void ActivateSubmitBtn(bool enable);
    public void SubmitAction();
    public void UpdateScore();
}

public class GameController_Anagram : MonoBehaviour,IBox
{
    [SerializeField] private RectTransform lettersParent;
    HorizontalLayoutGroup layoutGroup;

    [SerializeField] private GramBox letter;
    
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

    public IAnagramControl callback;

    #region LOCAL_VARIABLES
    [SerializeField] int currentQIndex = 0;
    public int CurrentQIndex => currentQIndex;

    bool isWon = false;

    #endregion


    [Header("Screen Orintation:")]
    [SerializeField] ScreenOrient screenOrient;
    public ScreenOrient ScreenOrient => screenOrient;

    private void OnEnable()
    {
        layoutGroup = lettersParent.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.enabled = true;

        styleUI = spriteData.GetStyle(UserData.currentAge);

        currentQIndex = 0;
        SetData();
    }

    private void OnDisable()
    {
      
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
                return;
            }

            GameData.levelID = res.ResponseData.levelData._id;

            questions = res.ResponseData.resArr;
            GameInitilise(questions[currentQIndex]);

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
            callback.ActivateSubmitBtn(true);
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

            box.SetImage(styleUI.box);
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


    IEnumerator ArrangeLetters(System.Action action)
    { 
        yield return null;
       

        action();
    }

    public void UpdatedPos(int index, Vector2 position)
    {
        GramBox currentBox =  gramBoxes.Find(x => x.Index == index && x.IsEnable && !x.isCorrect);

        List<GramBox> otherBoxes = gramBoxes.Where(p => p != currentBox  && p.IsEnable && !p.IsDrag && !p.isCorrect).ToList();

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