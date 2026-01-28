using BibleGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] Button backButton;


    [Header("Panels")]
    [SerializeField] GameObject _editPanel;
    [SerializeField] GameObject _termsPanel;
    [SerializeField] GameObject _logoutPanel;
    [SerializeField] GameObject _deletePanel;

    /// <summary>
    /// Action implemented on enable
    /// </summary>
    private void OnEnable()
    {
        backButton.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.setPanel); });

        _editPanel.GetComponent<EditPanel>().CloseAction.AddListener(()=> { AudioManager.Instance.PlayButton(); this.gameObject.SetActive(false); });
        _termsPanel.GetComponent<TermsConditionPanel>().CloseAction.AddListener(() => { AudioManager.Instance.PlayButton(); this.gameObject.SetActive(false); } );
        _logoutPanel.GetComponent<LogoutPanel>().CloseAction.AddListener(() => { AudioManager.Instance.PlayButton(); this.gameObject.SetActive(false); });
        _deletePanel.GetComponent<DeletePanel>().CloseAction.AddListener(() => { AudioManager.Instance.PlayButton(); this.gameObject.SetActive(false); });
    }

    /// <summary>
    /// Action impelemted on disable
    /// </summary>
    private void OnDisable()
    {
        backButton.onClick.RemoveAllListeners();
    }


    public void OpenSetPanel(SetPanelType type)
    {
        switch(type)
        {
            case SetPanelType.myProfile: _editPanel.SetActive(true); break;
            case SetPanelType.termsCondition: _termsPanel.SetActive(true); break;
            case SetPanelType.logout: _logoutPanel.SetActive(true); break;
            case SetPanelType.delete: _deletePanel.SetActive(true); break;
        };
    }

}
