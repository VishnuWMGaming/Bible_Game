using NativeTextToSpeech;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextSpeech : MonoBehaviour
{
    [SerializeField] TMP_Text mtext;

    private TextToSpeech _textToSpeech;

    Button mButton;

    UnityEvent FinishEvent;

    private void OnEnable()
    {
        mButton = this.GetComponent<Button>();
        mButton?.onClick.AddListener(Speak);

        _textToSpeech = TextToSpeech.Create(OnFinish, OnError);
    }

    private void OnDisable()
    {
        mButton?.onClick.RemoveAllListeners();
    }


    public void Speak()
    {
        if(String.IsNullOrWhiteSpace(mtext.text))
            return;

        _textToSpeech.Speak(mtext.text, "en-US", float.Parse("0.8", CultureInfo.InvariantCulture));
    }

    private void OnFinish()
    {
        FinishEvent?.Invoke();
    }

    private void OnError(string msg)
    {
        
    }
}
