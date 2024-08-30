using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using DG.Tweening;
using UnityEngine.EventSystems;


[RequireComponent(typeof(Button))]
public class UIButton : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] TMP_Text buttonText;
    public TMP_Text ButtonText =>  buttonText;

    Button button;
    public Button GetButton => button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    #region EVENT_FUNCTIONS
    /// <summary>
    /// Action implemented on jhighlight
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
      
    }

    /// <summary>
    /// Action implemented on not highlight
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
       
    }
    #endregion
}
