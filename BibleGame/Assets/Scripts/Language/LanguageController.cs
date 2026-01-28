using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using DebugUtils;
using Lean.Localization;
using RestAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageController : MonoBehaviour
{
    static LanguageController instance;

    public static LanguageController Instance { get { return instance; } }


    public Dictionary<string, string> Languages;

    [Header("Special Fonts:")]
    [SerializeField] TMP_FontAsset chineseFontAsset;
    public TMP_FontAsset ChineseFont => chineseFontAsset;


    [SerializeField] TMP_FontAsset koreanFontAsset;
    public TMP_FontAsset KoreanFont => koreanFontAsset;

    [SerializeField] TMP_FontAsset normalFontAsset;
    public TMP_FontAsset NormalFont => normalFontAsset;

    [SerializeField] TMP_FontAsset hindiFontAsset;
    public TMP_FontAsset HindiFont => hindiFontAsset;

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

        ChangeAllFonts();
    }

    public void SetLanguage(Language lang)
    {
        AppData.mLanguage = lang;
        PlayerPrefs.SetString("Language", lang.ToString());

        ChangeAllFonts();

        Actions.UpdateText(() =>
        {

        });
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

    public string GetLangVoiceCode(Language lang)
    {
        if (lang == null)
        {
            return "en-US";
        }

        string selectedLanguage = lang switch
        {
            Language.English => "en-US",
            Language.Spanish => "es-ES",
            Language.Korean => "ko-KR",
            Language.Portuguese => "pt-PT",
            Language.French => "fr-CA",
            Language.Chinese => "zh-CN",
            Language.Hindi => "hi_IN",
            Language.Swahili => "sw",
            Language.Kreyol => "en-US",
            _ => "en-US",
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

    public void Translate(string texValue, Action<string> onCompleted)
    {
        if (String.IsNullOrEmpty(ApiBase.AuthKeyPair.Value))
        {
            onCompleted?.Invoke(texValue);
            return;
        }


        if (AppData.mLanguage == null)
        {
            AppData.mLanguage = Language.English;
            onCompleted?.Invoke(texValue);
            return;
        }

        if (AppData.mLanguage == Language.English)
        {
            onCompleted?.Invoke(texValue);
            return;
        }

            //Split long text into chunks 
        List<string> chunks = SplitIntoChunks(texValue, 500);

        StartCoroutine(TranslateChunks(chunks, onCompleted));
    }

    public void ChangeAllFonts()
    {
        TMP_FontAsset newFont = AppData.mLanguage switch
        {
            Language.English => normalFontAsset,
            Language.Korean => koreanFontAsset,
            Language.Chinese => chineseFontAsset,
            Language.Hindi => hindiFontAsset,
            _=> normalFontAsset
        };

        // UI Text
        TextMeshProUGUI[] allTMP = FindObjectsOfType<TextMeshProUGUI>();
        foreach (var tmp in allTMP)
        {
            tmp.font = newFont;
        }

        // 3D Text
        TextMeshPro[] allTMP3D = FindObjectsOfType<TextMeshPro>();
        foreach (var tmp3D in allTMP3D)
        {
            tmp3D.font = newFont;
        }
    }

    private IEnumerator TranslateChunks(List<string> chunks, Action<string> callback)
    {
        string _translated_Text = "";
        string translation = "";

        foreach (var chunk in chunks)
        {
            bool done = false;

            string selectedLanguage = LanguageController.Instance.GetLangStringVal(AppData.mLanguage);
          

            TransInput input = new TransInput
            {
                text = chunk,
                language = selectedLanguage
            };

            TranslateAPI.Translate((success, res) =>
            {
                if (!success)
                {
                    Debug.LogError("Transaltion error");

                }
                done = true;

                string translated = res.ResponseData;

                if (translation != translated)
                {
                    _translated_Text += translated + " ";
                    translation = translated;

                    done = true;
                }
            }, input);

            // Wait until translation finished before sending next chunk
            yield return new WaitUntil(() => done);

            callback(_translated_Text);
        }
    }


    private IEnumerator TranslateChunks(List<string> chunks)
    {
       string _translated_Text = "";

        foreach (var chunk in chunks)
        {
            bool done = false;


            string selectedLanguage = LanguageController.Instance.GetLangStringVal(AppData.mLanguage);


            TransInput input = new TransInput
            {
                text = chunk,
                language = selectedLanguage
            };

            TranslateAPI.Translate((success, res) =>
            {
                if (!success)
                {
                    Debug.LogError("Transaltion error");

                }
                done = true;

                string translated = res.ResponseData;

                if (_translated_Text != translated)
                {
                    _translated_Text = translated;
                    done = true;
                }
            }, input);

            //     TranslateAPI.Translate.()
            // {
            //         translatedText.text += translated + " "; // append to final text
            //         _translated_Text += translated + " ";
            //         done = true;
            //     });

            // Wait until translation finished before sending next chunk
            yield return new WaitUntil(() => done);
        }
    }

    private List<string> SplitIntoChunks(string text, int maxChunkSize)
    {
        List<string> chunks = new List<string>();

        for (int i = 0; i < text.Length; i += maxChunkSize)
        {
            int length = Mathf.Min(maxChunkSize, text.Length - i);
            chunks.Add(text.Substring(i, length));
        }

        return chunks;
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
