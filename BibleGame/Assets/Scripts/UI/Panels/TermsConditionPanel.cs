using BibleGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TermsConditionPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] Button backBtn;

   public UnityEvent CloseAction;

    private void OnEnable()
    {
        backBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.setPanel); });
        backBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); CloseAction?.Invoke(); });   
        backBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); gameObject.SetActive(false); });
    }

    private void OnDisable()
    {
        backBtn.onClick.RemoveAllListeners();   
    }
}
