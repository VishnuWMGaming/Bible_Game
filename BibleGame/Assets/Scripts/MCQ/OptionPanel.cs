using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IOptionPanel
{
    public void CorrectAnswerAction();

    public void EnableSubmit(bool enabled);
}


public class OptionPanel : MonoBehaviour, IOption
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
            options[i].SetDisplay(Option.OptionType.normal);
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
        for(int i = 0;i<options.Count;++i)
        {
            if (options[i].Index != index)
                options[i].SetDisplay(Option.OptionType.normal);
        }

       _currentIndex = index;

        callback.EnableSubmit(true);
    }

    public void CheckAnswerAction()
    {
        OptionLockAction(true);

        if (_currentIndex == correctIndex)
        {
            Debug.LogWarning("CORRECT!!!!!!!!!!");
            options[correctIndex].SetDisplay(Option.OptionType.correct);

            callback.EnableSubmit(false);

            //callback.CorrectAnswerAction();
            Invoke("CorrectAction", 2.0f);
        }
        else
        {
            Debug.LogError("WRONG!!!!!!!!!!");
            options[_currentIndex].SetDisplay(Option.OptionType.worng);

            callback.EnableSubmit(false);

            StartCoroutine(RestartAction());
        }
    }

    IEnumerator RestartAction()
    {
        yield return new WaitForSeconds(2.0f);

        for (int i = 0; i < options.Count; i++)
        {
            options[i].EnableButtonInteraction(true);
            options[i].SetDisplay(Option.OptionType.normal);
        }

        options[0].SetChoice("1.Bird", 0);
        options[1].SetChoice("2.Cat", 1);
        options[2].SetChoice("3.Dog", 2);
        options[3].SetChoice("4.Fish", 3);
    }


    void CorrectAction()
    {
        callback.CorrectAnswerAction();
    }

    /// <summary>
    /// To lock all the options
    /// </summary>
    public void OptionLockAction(bool enable)
    {
        for (int i = 0; i < options.Count; i++)
            options[i].EnableButtonInteraction(!enable);
    }
}
