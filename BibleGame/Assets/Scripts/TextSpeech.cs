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

    [SerializeField] Animator animator;

    Button mButton;

    UnityEvent FinishEvent;

    private void OnEnable()
    {
        mButton = this.GetComponent<Button>();
        mButton?.onClick.AddListener(Speak);

        ResetAction();

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

        animator.enabled = true;
        animator.speed = 1.0f;
        _textToSpeech.Speak(mtext.text, "en-US", float.Parse("0.8", CultureInfo.InvariantCulture));
    }

    private void OnFinish()
    {
        ResetAction();
        FinishEvent?.Invoke();
    }

    private void OnError(string msg)
    {
        
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
    }
}
