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
        backBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.setPanel));
        backBtn.onClick.AddListener(() => CloseAction?.Invoke());   
        backBtn.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void OnDisable()
    {
        backBtn.onClick.RemoveAllListeners();   
    }
}
