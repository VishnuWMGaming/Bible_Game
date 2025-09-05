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

    [Header("Volume")]
    [SerializeField] Slider bgSlider;
    [SerializeField] Slider sfxSlider;

    /// <summary>
    /// Action implemented on enable
    /// </summary>
    private void OnEnable()
    {
        myProfileButton.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); EnablePanel(SetPanelType.myProfile);});
        termsConditionButton.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); EnablePanel(SetPanelType.termsCondition); });
        logoutButton.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); EnablePanel(SetPanelType.logout); });
        homeButton.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.home); });

        if (PlayerPrefs.HasKey("SfxVol"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SfxVol");
        }
        else
            sfxSlider.value = 1.0f;
        
        sfxSlider?.onValueChanged.AddListener(AudioManager.Instance.ChangeVolSfx);

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

        sfxSlider?.onValueChanged.RemoveAllListeners();
    }

    private void EnablePanel(SetPanelType type)
    {
        _setPanel.gameObject.SetActive(true);
        _setPanel.OpenSetPanel(type);

        this.gameObject.SetActive(false);
    }

}

public enum  SetPanelType { myProfile, termsCondition , logout}
