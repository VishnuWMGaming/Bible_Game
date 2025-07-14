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


public interface IAnagramControl
{
    public void ActivateSubmitBtn(bool enable);
    public void SubmitAction();
    public void UpdateScore();
}

public class GameController_Anagram : MonoBehaviour,IBox
{
    [SerializeField] private Transform lettersParent;
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

    public IAnagramControl callback;

    #region LOCAL_VARIABLES
    [SerializeField] int currentQIndex = 0;
    public int CurrentQIndex => currentQIndex;

    #endregion

    private void OnEnable()
    {
        lettersParent.gameObject.GetComponent<HorizontalLayoutGroup>().enabled = true;

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

            questions = res.ResponseData.questions;
            GameInitilise(questions[currentQIndex]);

            currentQIndex++;

        }, requestData);

    }

    public void NextQAct()
    {
        if(currentQIndex > questions.Count)
        {
            Debug.Log("Get read to submit !!!");
            callback.SubmitAction();
            return;
        }

        GameInitilise(questions[currentQIndex]);

        currentQIndex++;
    }

    void GameInitilise(GetQuestionsAnagramData question)
    {
        string scrambledword = question.title;
        string[] letterVals = scrambledword.Select(c => c.ToString()).ToArray();

        for(int i = 0;i< letterVals.Length;++i)
        {
            AnagramLetter letter = new AnagramLetter
            {
                index = i,
                val = letterVals[i]
            };

            _anagramLetters.Add(letter);
        }

        Debug.Log($"Loading... scrambling");

        foreach (var gramBox in gramBoxes)
        {
            Destroy(gramBox);
        }
        gramBoxes.Clear();
        //var shuffledList = GetShuffledList(anagramLs[1].AnagramLetters);

        for (var index = 0; index < _anagramLetters.Count/*shuffledList.Count*/; index++)
        {
            var letters = _anagramLetters[index]/*shuffledList[index]*/;
            var temp = Instantiate(letter, lettersParent);

            temp.SetLetter(letters.val);
            temp.SetIndex(letters.index);

            temp.maskArea = lettersParent.GetComponent<RectTransform>();

            temp.SetImage(styleUI.box);

            gramBoxes.Add(temp);
        }

        StartCoroutine(DisableHorizontalLayoutGroup());

        for (int i = 0; i < gramBoxes.Count; ++i)
            gramBoxes[i].callback = this;

        Debug.Log($"Loading... answer");

        _anagramLetters.Clear();

        string rightword = question.hint;
        string[] lettervals = rightword.Select(c => c.ToString()).ToArray();

        for (int i = 0; i < lettervals.Length; ++i)
        {
            AnagramLetter letter = new AnagramLetter
            {
                index = i,
                val = lettervals[i]
            };

            _anagramLetters.Add(letter);
        }
    }


    void ShuffleGramboxValues(List<GramBox> objects)
    {
        if (objects == null || objects.Count == 0) return;

        // Create a list of indexes
        List<string> shuffledValues = new List<string>();
        for (int i = 0; i < objects.Count; i++)
        {
            shuffledValues.Add(objects[i].Value);
        }

        // Shuffle the indexes
        for (int i = shuffledValues.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (shuffledValues[i], shuffledValues[randomIndex]) = (shuffledValues[randomIndex], shuffledValues[i]);
        }

        // Assign shuffled sibling indexes
        for (int i = 0; i < objects.Count; i++)
        {
            // objects[i].gameObject.transform.SetSiblingIndex(siblingIndexes[i]);
            objects[i].SetLetter(shuffledValues[i]);
        }
    }

    IEnumerator DisableHorizontalLayoutGroup()
    {
        yield return new WaitForEndOfFrame();
        ShuffleGramboxValues(gramBoxes);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        lettersParent.gameObject.GetComponent<HorizontalLayoutGroup>().enabled = false;
        yield return new WaitForEndOfFrame();
        foreach (var gramBox in gramBoxes)
        {
            gramBox.SetStartPos(gramBox.transform.localPosition);
        }
        // yield return new WaitForSeconds(10f);
        yield return new WaitForEndOfFrame();
        foreach (var gramBox in gramBoxes)
        {
            gramBox.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            gramBox.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            gramBox.transform.localPosition = gramBox.GetStartPos();
        }
        yield return new WaitForEndOfFrame();
        // yield return new WaitForSeconds(10f);
        foreach (var gramBox in gramBoxes)
        {
            gramBox.gameObject.SetActive(false);
        }
        foreach (var gramBox in gramBoxes)
        {
            gramBox.gameObject.SetActive(true);
        }
    }

    public void UpdatedPos(int index, Vector2 position)
    {
        GramBox currentBox =  gramBoxes.Find(x => x.Index == index);

        foreach(GramBox box in gramBoxes)
        {
            if (index == box.Index)
                continue;

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
        bool isWon = false;

        Debug.Log("End game");

        for (int i = 0; i < gramBoxes.Count; ++i)
        {
            isWon = gramBoxes[i].isCorrect;

            if (!isWon)
                break;
        }

        congratsTxt.SetActive(isWon);

        callback.ActivateSubmitBtn(isWon);

        if (isWon)
          Debug.LogWarning("Got the word");
            
    }

    IEnumerator CheckingResult(int index)
    {
        if (index > letterCount)
        {
            StopAllCoroutines();
            EndGame(); 
        }
        else
        {
            for (int i = 0; i < gramBoxes.Count; ++i)
            {
                if (index == gramBoxes[i].Index)
                {
                    result = result + gramBoxes[i].Value;
                    gramBoxes[i].SetCorrectWord(_anagramLetters.Find(x => x.index == index).val == gramBoxes[i].Value);
                    Debug.Log("Checking .... " + index);
                    
                    Debug.Log($"Checking 2 .... {_anagramLetters.Find(x => x.index == index).val}  {gramBoxes[i].Value}" );
                    
                    index++;
                    StartCoroutine(CheckingResult(index));
                }
            }
        }

        yield return null;
    }
}

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