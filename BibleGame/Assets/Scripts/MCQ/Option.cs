using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using UnityEngine.EventSystems;
using UnityEngine.Events;


public interface IOption
{
    public void OptionSelected(int index);  
}

[RequireComponent(typeof(Button))]
public class Option : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private OptionColors optionColors;
    [SerializeField]  Button button;

    public enum OptionType {normal,selected,correct, wrong,disable};

    [Header("OptionType:")]
    [Tooltip("it gives the current state of the option")]
    [SerializeField] OptionType _optionType;
    public OptionType CurrentOption => _optionType;

    [Space]
    [SerializeField] OptionHighlight optionHighlight;

    [Header("Index:")]
    [SerializeField] int _index;
    public int Index => _index;

    [Header("Text:")]
    [SerializeField] TMP_Text optionText;

    public IOption callback;

    bool isEnable = true;

    string mValue;
    public string Val => mValue;

    [Header("Speaker:")]
    [SerializeField] TextSpeech speech;

    [Header("Translate")]
    [SerializeField] TranslateLang translateLang;

    public void EnableInteractable(bool enable) { button.interactable = enable; }

    /// <summary>
    /// Action implemented on enable
    /// </summary>
    private void OnEnable()
    {
        button.onClick.AddListener(() => { SelectAction(); AudioManager.Instance.PlayButton(); });

        optionText = this.GetComponentInChildren<TMP_Text>();
 
        EnableButtonInteraction(true);
        SetDisplay(OptionType.normal);

      
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        if (button != null) 
            button.onClick.RemoveAllListeners();
    }

    public void EnableButtonInteraction(bool enable)
    {
        button.interactable = enable;
        isEnable = enable;
    }

    public void SetChoice(string option ,int index)
    {
        mValue = option;

        optionText.text = "";

        LanguageController.Instance.Translate(option, (translation) =>
        {
            optionText.text = translation;
            _index = index;

            speech.Initialise(translation);
        });

      
    }

    #region EVENT_FUNCTIONS
    /// <summary>
    /// Action implemented on jhighlight
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        //if(isEnable)
        ////optionHighlight.gameObject.SetActive(true);
    }

    /// <summary>
    /// Action implemented on not highlight
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
       // optionHighlight.gameObject.SetActive(false);

    }
    #endregion

    public void SetOption(OptionType optionType)
    {
        _optionType = optionType;
    }

    public void SelectAction()
    {
        SetDisplay(OptionType.selected);
        speech.Speak();

        callback.OptionSelected(_index);
    }

    public void SetDisplay(OptionType optionType)
    {
        switch(optionType)
        {
            case OptionType.normal:
                optionHighlight.gameObject.SetActive(false);
                button.image.color = new Color32(54, 188, 228, 255);
                break;

            case OptionType.selected:
                optionHighlight.gameObject.SetActive(true);
                optionHighlight.ChangeSet(OptionType.normal);
                 break;

            case OptionType.wrong:
                optionHighlight.gameObject.SetActive(true);
                button.image.color = new Color32(254,6,6,255);

                optionHighlight.ChangeSet(OptionType.wrong);
                break;

             case OptionType.correct:
                optionHighlight.ChangeSet(OptionType.correct);
                optionHighlight.gameObject.SetActive(true);

              //  button.image.color = new Color32(39,163,38,255);

                break;

            case OptionType.disable:
                optionHighlight.gameObject.SetActive(true);
                optionHighlight.ChangeSet(OptionType.disable);
                

                break;
            
        }
    }
}
