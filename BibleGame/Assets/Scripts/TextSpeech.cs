using NativeTextToSpeech;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Windows;

public class TextSpeech : MonoBehaviour
{
    [SerializeField] TMP_Text mtext;

    private TextToSpeech _textToSpeech;

    [SerializeField] Animator animator;

    Button mButton;

    UnityEvent FinishEvent;

    bool isPlaying = false;

    private void OnEnable()
    {
        mButton = this.GetComponent<Button>();
        mButton?.onClick.AddListener(() => { Speak(); AudioManager.Instance.PlayButton(); });

        mtext = null;
        mtext = this.GetComponentInParent<TMP_Text>(); 

        ResetAction();

        _textToSpeech = TextToSpeech.Create(OnFinish, OnError);
    }

    public void Initialise(TMP_Text text,bool isButton = false)
    {
        mtext = text;

        mButton.interactable = isButton;
    }

    private void OnDisable()
    {
        mButton?.onClick.RemoveAllListeners();
    }


    public void Speak()
    {
        if(isPlaying)
        {
            Stop();
            return;
        }

        if( mtext == null ||  String.IsNullOrWhiteSpace(mtext.text))
            return;

        string filtered = Regex.Replace(mtext.text, @"[^a-zA-Z\s]", "");

        filtered = Regex.Replace(filtered, @"\s+", " ");
        filtered = filtered.Trim();

        Debug.Log($"<color=magenta> Speaking..... {filtered} </color>");

        animator.enabled = true;
        animator.speed = 1.0f;

        _textToSpeech.Speak(filtered, "en-US", float.Parse("0.8", CultureInfo.InvariantCulture));

        isPlaying = true;
    }

    private void OnFinish()
    {
        ResetAction();
        FinishEvent?.Invoke();
    }

    private void OnError(string msg)
    {
        Debug.LogError($"Error in speech: " + msg);
    }

    public void Stop()
    {
        ResetAction();
        _textToSpeech.Stop();
    }

    void ResetAction()
    {
        AnimationClip clip = animator.runtimeAnimatorController.animationClips[0];

        // Jump to 1 second into the clip
        float normalizedTime = 1f / clip.length;
        animator.Play(0, 0, normalizedTime);

        // Freeze so it stays there
        animator.speed = 0f;
        animator.enabled = false;

        isPlaying = false;
    }
}
