using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using UnityEngine.EventSystems;
using UnityEngine.Events;


public interface IOption
{
    public void OptionLockAction();
}

[RequireComponent(typeof(Button))]
public class Option : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    Button button;

    #region EVENTS
    [HideInInspector]
    public UnityEvent OnHightlight;
    [HideInInspector]
    public UnityEvent OnNotHighlight;
    #endregion

    public enum OptionType {normal,selected,correct, worng};

    [Header("OptionType:")]
    [Tooltip("it gives the current state of the option")]
    [SerializeField] OptionType optionType;
    public OptionType CurrentOption => optionType;

    [Space]
    [SerializeField] OptionHighlight optionHighlight;


    TMP_Text optionText;

    public IOption callback;

    /// <summary>
    /// Action implemented on enable
    /// </summary>
    private void OnEnable()
    {
        button = GetComponent<Button>();
        optionText = this.GetComponentInChildren<TMP_Text>();

        EnableButtonInteraction(true);
    }

    public void EnableButtonInteraction(bool enable)
    {
        button.interactable = enable;
    }

    public void SetChoice(string option)
    {
        optionText.text = option;
    }

    #region EVENT_FUNCTIONS
    /// <summary>
    /// Action implemented on jhighlight
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        optionHighlight.gameObject.SetActive(true);

        OnHightlight?.Invoke();
    }

    /// <summary>
    /// Action implemented on not highlight
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        optionHighlight.gameObject.SetActive(false);

        OnNotHighlight?.Invoke();
    }
    #endregion

    public void SetOption(OptionType optionType)
    {
        switch(optionType)
        {
            case OptionType.normal:
                optionHighlight.ChangeSet(OptionType.worng);
                optionHighlight.gameObject.SetActive(false);
                button.image.color = new Color32(54, 188, 228, 255);
                break;

            case OptionType.selected:
                optionHighlight.ChangeSet(OptionType.worng);
                optionHighlight.gameObject.SetActive(true); 
                 break;

            case OptionType.worng:
                optionHighlight.gameObject.SetActive(false);
                button.image.color = new Color32(254,6,6,255);
                break;

             case OptionType.correct:
                optionHighlight.gameObject.SetActive(true);
                optionHighlight.ChangeSet(optionType);
                button.image.color = new Color32(39,163,38,255);

                callback.OptionLockAction();
                break;
            
        }
    }
}
