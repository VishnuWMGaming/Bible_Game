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

[RequireComponent(typeof(TMP_Text))]
public class TranslateLang : MonoBehaviour
{
    [SerializeField] TMP_Text translatedText;
    [SerializeField] string _translated_Text;
    // Start is called before the first frame update
    // private string textValue;

    private void Awake()
    {
        translatedText = GetComponent<TMP_Text>();
        // textValue = translatedText.text;
        UpdateText();
    }

    private void OnEnable()
    {
        Actions.UpdateText += UpdateText;
    }
    private void OnDisable()
    {
        Actions.UpdateText -= UpdateText;
    }

    private void UpdateText()
    {
        string currentValue = translatedText.text;
        if (string.IsNullOrEmpty(currentValue))
            return;

        //Split long text into chunks 
        List<string> chunks = SplitIntoChunks(currentValue, 500);
        StartCoroutine(TranslateChunks(chunks));

        // // LanguageController.Instance.Translation(textValue, translated =>
        // LanguageController.Instance.Translation(currentValue, translated =>
        // {
        //     // Debug.LogError(translated);
        //     this.translatedText.text = translated;
        //     this._translated_Text = translated;
        // });
    }

    private IEnumerator TranslateChunks(List<string> chunks)
    {
        translatedText.text = " ";
        foreach (var chunk in chunks)
        {
            bool done = false;

            string selectedLanguage = LanguageController.Instance.GetLangStringVal(AppData.mLanguage);


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

                }
                done = true;

                string translated = res.ResponseData;
                translatedText.text += translated + " ";
                _translated_Text += translated + " ";
                done = true;

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
