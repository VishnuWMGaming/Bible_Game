using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using UnityEngine.UIElements;
using System;
using System.Reflection;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController_Anagram : MonoBehaviour,IBox
{
    [SerializeField] private Transform lettersParent;
    [SerializeField] private GramBox letter;
    
    [Header("Boxes:")]
    [SerializeField] List<GramBox> gramBoxes = new List<GramBox>();

    [SerializeField] List<AnagramLetterList> anagramLs = new List<AnagramLetterList>();

    string result = string.Empty;

    [SerializeField] GameObject congratsTxt;

    [Header("letterCount")]
    [SerializeField] int letterCount;

    [Header("Current Word Index"), SerializeField]
    private int currentWordIndex = 0;

    public IAnagramManager iAnagramManagerCallback;

    private void Start()
    {
        iAnagramManagerCallback = this.GetComponent<AnagramManager>();
        lettersParent.gameObject.GetComponent<HorizontalLayoutGroup>().enabled = true;
        
        foreach (var gramBox in gramBoxes)
        {
            Destroy(gramBox);
        }
        gramBoxes.Clear();
        //var shuffledList = GetShuffledList(anagramLs[1].AnagramLetters);
        
        for (var index = 0; index < anagramLs[0].AnagramLetters.Count/*shuffledList.Count*/; index++)
        {
            var letters = anagramLs[0].AnagramLetters[index]/*shuffledList[index]*/;
            var temp = Instantiate(letter, lettersParent);
            
            temp.SetLetter(letters.val);
            temp.SetIndex(letters.index);

            temp.maskArea = lettersParent.GetComponent<RectTransform>();
            
            gramBoxes.Add(temp);
        }

        StartCoroutine(DisableHorizontalLayoutGroup());

        for (int i = 0; i < gramBoxes.Count; ++i)
            gramBoxes[i].callback = this;
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
        int index = 1;
        StartCoroutine(CheckingResult(index));
    }


    void EndGame()
    {
        bool isWon = false;

        for (int i = 0; i < gramBoxes.Count; ++i)
        {
            isWon = gramBoxes[i].isCorrect;

            if (!isWon)
                break;
        }

        congratsTxt.SetActive(isWon);

        if (isWon)
        {
            iAnagramManagerCallback.ActivateSubmitBtn();
            Debug.LogWarning("Got the word");
        }
        else
        {
            iAnagramManagerCallback.DeactivateSubmitBtn();
        }
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
                    gramBoxes[i].SetCorrectWord(anagramLs[currentWordIndex].AnagramLetters.Find(x => x.index == index).val == gramBoxes[i].Value);
                    Debug.Log("Checking .... " + index);
                    
                    Debug.Log($"Checking 2 .... {anagramLs[currentWordIndex].AnagramLetters.Find(x => x.index == index).val}  {gramBoxes[i].Value}" );
                    
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