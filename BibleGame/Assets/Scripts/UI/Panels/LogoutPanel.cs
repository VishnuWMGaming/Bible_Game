using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BibleGame;
using UnityEngine.Events;

public class LogoutPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] Button yesBtn;
    [SerializeField] Button noBtn;
    [SerializeField] Button closeBtn;

    public UnityEvent CloseAction;

    private void OnEnable()
    {
        yesBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.login));
        yesBtn.onClick.AddListener(() => CloseAction?.Invoke());
        yesBtn.onClick.AddListener(() => gameObject.SetActive(false));  

        noBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.setPanel));
        noBtn?.onClick.AddListener(() => CloseAction?.Invoke());
        noBtn.onClick.AddListener(() => gameObject.SetActive(false));

        closeBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.setPanel));
        closeBtn.onClick.AddListener(() => CloseAction?.Invoke());
        closeBtn.onClick.AddListener(() => gameObject.SetActive(false));    
    }

    private void OnDisable()
    {
        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
    }
}
