using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] Button myProfileButton;
    [SerializeField] Button termsConditionButton;
    [SerializeField] Button logoutButton;

    [SerializeField] SetPanel _setPanel;


    /// <summary>
    /// Action implemented on enable
    /// </summary>
    private void OnEnable()
    {
        myProfileButton.onClick.AddListener(() => EnablePanel(SetPanelType.myProfile));
        termsConditionButton.onClick.AddListener(() => EnablePanel(SetPanelType.termsCondition));
        logoutButton.onClick.AddListener(() => EnablePanel(SetPanelType.logout));
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        myProfileButton.onClick.RemoveAllListeners();
        termsConditionButton.onClick.RemoveAllListeners();
        logoutButton.onClick.RemoveAllListeners();
    }

    private void EnablePanel(SetPanelType type)
    {
        _setPanel.gameObject.SetActive(false);
        _setPanel.OpenSetPanel(type);
    }

}

public enum  SetPanelType { myProfile, termsCondition , logout}
