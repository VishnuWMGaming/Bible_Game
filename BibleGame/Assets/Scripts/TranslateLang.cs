using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BibleGame;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TranslateLang : MonoBehaviour
{
    [SerializeField] TMP_Text translatedText ;
    [SerializeField] string _translated_Text ;
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
        if(string.IsNullOrEmpty(currentValue))
            return;
        
        // LanguageController.Instance.Translation(textValue, translated =>
        LanguageController.Instance.Translation(currentValue, translated=>
        {
            // Debug.LogError(translated);
            this.translatedText.text = translated;
            this._translated_Text = translated;
        });
    }
}
