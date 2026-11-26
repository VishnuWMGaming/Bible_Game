using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Unity.VisualScripting;

public interface IUIButton
{
    public void SelectAction(UIButton iButton);
}


[RequireComponent(typeof(Button))]
public class UIButton : MonoBehaviour
{
    [SerializeField] TMP_Text buttonText;
    public TMP_Text ButtonText => buttonText;

    Button button;

    [Header("UI Settings")]
    [SerializeField] bool isSetAct = false;
    [SerializeField] Sprite mSelectedImg;
    [SerializeField] Sprite mUnSelectedImg;

    [Space]
    [SerializeField] UnityEvent mClickAction;


    public IUIButton callback;

    public Button GetButton => button;

    private void Awake()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayButton();

            if (isSetAct)
            {
                button.image.sprite = mSelectedImg;
            }

            mClickAction?.Invoke();

            callback?.SelectAction(this);
        });
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    public void UnSelectAction()
    {
        button.image.sprite = mUnSelectedImg;
    }

    #region EVENT_FUNCTIONS
   
    #endregion
}
