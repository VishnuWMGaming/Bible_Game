using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BibleGame;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using BibleGame.API;
using BibleGame.Data;
using RestAPI;
using DebugUtils;

[RequireComponent(typeof(TMP_Text))]
public class TranslateLang : MonoBehaviour
{
    [SerializeField] TMP_Text translatedText;
    [SerializeField] string _translated_Text;
    // Start is called before the first frame update
     private string textValue;


    [SerializeField] bool isTextChange = true;

    private void Awake()
    {
        translatedText = GetComponent<TMP_Text>();
        textValue = translatedText.text;

        UpdateText(() =>
        {

        });
    }


    private void OnEnable()
    {
        translatedText.font = AppData.mLanguage switch
        {
            Language.English => LanguageController.Instance.NormalFont,
            Language.Korean => LanguageController.Instance.KoreanFont,
            Language.Chinese => LanguageController.Instance.ChineseFont,
            Language.Hindi => LanguageController.Instance.HindiFont,
            _ => LanguageController.Instance.NormalFont
        };


        UpdateText(() =>
        {

        });

        Actions.UpdateText += UpdateText;
    }
    private void OnDisable()
    {
        Actions.UpdateText -= UpdateText;
    }

    public void UpdateValue(string value)
    {
        textValue = value;
    }

    public void UpdateText(Action finish)
    {
        if (String.IsNullOrEmpty(ApiBase.AuthKeyPair.Value))
        {
            finish?.Invoke();
            return;
        }

        string currentValue = translatedText.text;

        if (string.IsNullOrEmpty(currentValue))
        {
            finish?.Invoke();
            return;
        }

        if (AppData.mLanguage == null)
        {
            AppData.mLanguage = Language.English;

            finish?.Invoke();
            return;
        }

        if(AppData.mLanguage == Language.English)
        {
            translatedText.text = textValue;
            finish?.Invoke();
            return;
        }

       translatedText.font =  AppData.mLanguage switch
       {
           Language.English => LanguageController.Instance.NormalFont,
           Language.Korean => LanguageController.Instance.KoreanFont,
           Language.Chinese => LanguageController.Instance.ChineseFont,
           Language.Hindi => LanguageController.Instance.HindiFont,
           _ => LanguageController.Instance.NormalFont
       };


        if (!isTextChange)
            return;

        translatedText.text = "";

        if(textValue.Length <= 200)
        {
            LanguageController.Instance.Translate(textValue, (translation) =>
            {
                translatedText.text = translation;
                finish?.Invoke();
            });

            return;
        }


        //Split long text into chunks 
        List<string> chunks = SplitIntoChunks(textValue, 500);


        StartCoroutine(TranslateChunks(chunks, () =>
        {
            finish?.Invoke();
        }));

        // // LanguageController.Instance.Translation(textValue, translated =>
        // LanguageController.Instance.Translation(currentValue, translated =>
        // {
        //     // Debug.LogError(translated);
        //     this.translatedText.text = translated;
        //     this._translated_Text = translated;
        // });
    }

    private IEnumerator TranslateChunks(List<string> chunks,Action finish)
    {
        translatedText.text = " ";
        _translated_Text = "";

        foreach (var chunk in chunks)
        {
            bool done = false;

            
            string selectedLanguage = LanguageController.Instance.GetLangStringVal(AppData.mLanguage);
            DevDebug.Log($"Selected language: {selectedLanguage}",DebugColor.Silver);

            TransInput input = new TransInput
            {
                text = chunk,
                language =selectedLanguage
            };

            TranslateAPI.Translate((success,res) =>
            {
                if (!success)
                {
                    Debug.LogError("Transaltion error");
                    done = true;
                }

                string translated = res.ResponseData;

                if (_translated_Text != translated)
                {

                    translatedText.text += translated + " ";
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

            finish?.Invoke();
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
