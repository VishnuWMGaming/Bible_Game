using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using DebugUtils;
using JetBrains.Annotations;
using System;
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
    [SerializeField] Button deleteButton;
    [SerializeField] Button homeButton;
    [SerializeField] Button languageButton;
    [SerializeField] TMP_Text userName;
    [SerializeField] GameObject disablerPanel;

    [SerializeField] SetPanel _setPanel;

    [Header("Volume")]
    [SerializeField] Slider bgSlider;
    [SerializeField] Slider sfxSlider;
    public TMP_Dropdown languageDropdown;
    /// <summary>
    /// Action implemented on enable
    /// </summary>
    private void OnEnable()
    {
        IsUserLoggedIn();
        // Clear old options
        languageDropdown.ClearOptions();

        // Add new language options
        var options = new List<string>
        {
            "English",
            "Spanish",
            "Portuguese",
            "Korean",
            "French",
            "Chinese",
            "Hindi",
            "Swahili",
            "Kreyol"
        };

     languageDropdown.AddOptions(options);

        languageDropdown.value = PlayerPrefs.HasKey("lang") ? PlayerPrefs.GetInt("lang") : 0;
        myProfileButton.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); EnablePanel(SetPanelType.myProfile); });
        termsConditionButton.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); EnablePanel(SetPanelType.termsCondition); });
        logoutButton.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); EnablePanel(SetPanelType.logout); });
        homeButton.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.home); });

        deleteButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayButton();
            EnablePanel(SetPanelType.delete);
        });

        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        if (PlayerPrefs.HasKey("SfxVol"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SfxVol");
        }
        else
            sfxSlider.value = 1.0f;


        if (PlayerPrefs.HasKey("BGVol"))
        {
            bgSlider.value = PlayerPrefs.GetFloat("BGVol");
        }
        else
            bgSlider.value = 1.0f;

        sfxSlider?.onValueChanged.AddListener(AudioManager.Instance.ChangeVolSfx);

        bgSlider?.onValueChanged.AddListener(AudioManager.Instance.ChangeVolBG);

        userName.text = AppData.loginData.Name;
    }

    private void IsUserLoggedIn()
    {
        string token = PlayerPrefs.GetString("AuthorizationToken");
        if (string.IsNullOrEmpty(token))
        {
            myProfileButton.interactable = false;
            logoutButton.interactable=false;
            deleteButton.interactable=false;
            languageButton.interactable = false;
            disablerPanel.SetActive(true);
        }
        else
        {
            myProfileButton.interactable = true;
            logoutButton.interactable = true;
            deleteButton.interactable = true;
            languageButton.interactable = true;
            disablerPanel.SetActive(false);
        }
    }

    private void OnLanguageChanged(int languagecode)
    {
        // LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languagecode];
        PlayerPrefs.SetInt("lang", languagecode);
        LanguageController.Instance.SetLanguage((Language)languagecode);
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

public enum SetPanelType { myProfile, termsCondition, logout,delete }
