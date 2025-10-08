using BibleGame.Data;
using Lean.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BibleGame;

public class LanguageController : MonoBehaviour
{
    static LanguageController instance;

    public static LanguageController Instance { get { return instance; } }


    public Dictionary<string, string> Languages;

    [Header("Translator:")]
    [SerializeField] MyMemoryTranslator translator;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        Languages = new Dictionary<string, string>()
        {
            { "English", "en" },
            { "Spanish", "es" },
            { "Portuguese", "pt" },
            { "Korean", "ko" },
            { "French", "fr" },
            { "Chinese", "zh" },
            { "Hindi", "hi" },
            { "Swahili", "sw" },
            { "Kreyol", "ht" }
        };

        if (PlayerPrefs.HasKey("Language"))
        {
            string selectedlanguage = PlayerPrefs.GetString("Language");

            AppData.mLanguage = GetLang(selectedlanguage);
        }
        else
        {
            AppData.mLanguage = Language.English;
            PlayerPrefs.SetString("Language", "English");
        }
    }

    public void SetLanguage(Language lang)
    {
        AppData.mLanguage = lang;
        PlayerPrefs.SetString("Language", lang.ToString());
        Actions.UpdateText();
    }
    private void Start()
    {
        Translation("Greeting", translated =>
        {
            Debug.Log($"Translated :{translated}");
        });
    }
    public void Translation(string text, Action<string> onTranslated)
    {
        string translatedText = string.Empty;

        string selectedLanguage = GetLangStringVal(AppData.mLanguage);
        if (selectedLanguage == "English")
        {
            onTranslated?.Invoke(text);
            return;
        }

        if (Languages.TryGetValue(selectedLanguage, out string code))
        {
            StartCoroutine(translator.TranslateAndStore(text, "en", code, (translation) =>
            {
                onTranslated?.Invoke(translation);
            }));
        }
    }


    public string GetLangStringVal(Language lang)
    {
        string selectedLanguage = lang switch
        {
            Language.English => "en",
            Language.Spanish => "es",
            Language.Korean => "ko",
            Language.Portuguese => "pt",
            Language.French => "fr",
            Language.Chinese => "zh-CN",
            Language.Hindi => "hi",
            Language.Swahili => "sw",
            Language.Kreyol => "ht",
            _ => "en",
        };

        return selectedLanguage;
    }

    Language GetLang(string lang)
    {
        Language selLang = lang switch
        {
            "English" => Language.English,
            "Spanish" => Language.Spanish,
            "Portuguese" => Language.Portuguese,
            "Korean" => Language.Korean,
            "French" => Language.French,
            "Chinese" => Language.Chinese,
            "Hindi" => Language.Hindi,
            "Swahili" => Language.Swahili,
            "Kreyol" => Language.Kreyol,
            _ => Language.English
        };

        return selLang;
    }

}


public enum Language
{
    English,
    Spanish,
    Portuguese,
    Korean,
    French,
    Chinese,
    Hindi,
    Swahili,
    Kreyol
}
