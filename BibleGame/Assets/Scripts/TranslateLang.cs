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
    private string textValue;

    private void Awake()
    {
        translatedText = GetComponent<TMP_Text>();
        textValue = translatedText.text;
    }

    private void OnEnable()
    {
        Actions.UpdateText += UpdateText;
    }

    private void UpdateText()
    {
        LanguageController.Instance.Translation(textValue, translated =>
        {
            // Debug.LogError(translated);
            this.translatedText.text = translated;
            this._translated_Text = translated;
        });

    }

    private void OnDisable()
    {
        Actions.UpdateText -= UpdateText;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
