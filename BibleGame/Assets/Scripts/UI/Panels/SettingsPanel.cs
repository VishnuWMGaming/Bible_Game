using BibleGame;
using BibleGame.Data;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] Button myProfileButton;
    [SerializeField] Button termsConditionButton;
    [SerializeField] Button logoutButton;
    [SerializeField] Button homeButton;
    [SerializeField] TMP_Text userName;

    [SerializeField] SetPanel _setPanel;


    /// <summary>
    /// Action implemented on enable
    /// </summary>
    private void OnEnable()
    {
        myProfileButton.onClick.AddListener(() => EnablePanel(SetPanelType.myProfile));
        termsConditionButton.onClick.AddListener(() => EnablePanel(SetPanelType.termsCondition));
        logoutButton.onClick.AddListener(() => EnablePanel(SetPanelType.logout));
        homeButton.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.home));

        userName.text = AppData.loginData.Name;
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        myProfileButton.onClick.RemoveAllListeners();
        termsConditionButton.onClick.RemoveAllListeners();
        logoutButton.onClick.RemoveAllListeners();
        homeButton.onClick.RemoveAllListeners();
    }

    private void EnablePanel(SetPanelType type)
    {
        _setPanel.gameObject.SetActive(true);
        _setPanel.OpenSetPanel(type);

        this.gameObject.SetActive(false);
    }

}

public enum  SetPanelType { myProfile, termsCondition , logout}
