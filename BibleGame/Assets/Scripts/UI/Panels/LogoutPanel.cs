using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BibleGame;
using TMPro;
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
        yesBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); PlayerPrefs.DeleteAll(); });
        yesBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.login); });
        yesBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); CloseAction?.Invoke(); });
        yesBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); gameObject.SetActive(false); });  

        noBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.setPanel); }        );
        noBtn?.onClick.AddListener(()    => { AudioManager.Instance.PlayButton(); CloseAction?.Invoke(); });
        noBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); gameObject.SetActive(false); });

        closeBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.setPanel); });
        closeBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); CloseAction?.Invoke(); });
        closeBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); gameObject.SetActive(false); });    
    }

    

    private void OnDisable()
    {
        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
    }
}
