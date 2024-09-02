using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOptionPanel
{
    public void CorrectAnswerAction();
}


public class OptionPanel : MonoBehaviour,IOption
{
    [Header("Options:")]
    [SerializeField] List<Option> options = new List<Option>();

    int correctIndex = -1;
    int _currentIndex = -1;

    public IOptionPanel callback;

    private void OnEnable()
    {
        for (int i = 0; i < options.Count; i++)
        {
            options[i].callback = this;
            options[i].EnableButtonInteraction(true);
        }

        options[0].SetChoice("1.Bird",0);
        options[1].SetChoice("2.Cat",1);
        options[2].SetChoice("3.Dog",2);
        options[3].SetChoice("4.Fish",3);

        _currentIndex = -1;
        correctIndex = 0;    
    }

    public void OptionSelected(int index)
    {
       _currentIndex = index;
    }

    public void CheckAnswerAction()
    {
        OptionLockAction();

        if (_currentIndex == correctIndex)
        {
            Debug.LogWarning("CORRECT!!!!!!!!!!");
            options[correctIndex].SetDisplay(Option.OptionType.correct);

            //callback.CorrectAnswerAction();
            Invoke("CorrectAction", 2.0f);
        }
        else
        {
            Debug.LogError("WRONG!!!!!!!!!!");
            options[_currentIndex].SetDisplay(Option.OptionType.worng);
        }
    }


    void CorrectAction()
    {
        callback.CorrectAnswerAction();
    }

    /// <summary>
    /// To lock all the options
    /// </summary>
    public void OptionLockAction()
    {
        for (int i = 0; i < options.Count; i++)
            options[i].EnableButtonInteraction(false);
    }
}
