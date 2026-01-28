using BibleGame;
using BibleGame.API;
using DebugUtils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DeletePanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] Button yesBtn;
    [SerializeField] Button noBtn;
    [SerializeField] Button closeBtn;
    public UnityEvent CloseAction;

    private void OnEnable()
    {
        yesBtn.onClick.AddListener(() => 
        {
            AudioManager.Instance.PlayButton();
            PopUp.Instance.EnableLoad(true);

            ProfileAPI.DeleteProfile((success) =>
            {
                PopUp.Instance.EnableLoad(false);

                if (!success)
                {
                    Debug.LogError("Delete profile failed !!");
                    return;
                }

                DevDebug.Log("Deleted account successfully !!", DebugColor.Green);

                CloseAction?.Invoke();
                PlayerPrefs.DeleteAll();

                gameObject.SetActive(false);
                Actions.ChangePanelActions(CanvasType.login);
            });
        });

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
