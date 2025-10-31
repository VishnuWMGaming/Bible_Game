using BibleGame.Data;
using NativeTextToSpeech;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Windows;

public class TextSpeech : MonoBehaviour
{
    [SerializeField] string mtext;

    private TextToSpeech _textToSpeech;

    [SerializeField] Animator animator;

    Button mButton;

    Sprite mInitialSprite;

    UnityEvent FinishEvent;

    bool isPlaying = false;

    private void OnEnable()
    {
        mButton = this.GetComponent<Button>();
        mButton?.onClick.AddListener(() => { Speak(); AudioManager.Instance.PlayButton(); });

        mtext = null;
        //mtext = this.GetComponentInParent<TMP_Text>(); 

        mInitialSprite = mButton.image.sprite;

        ResetAction();

#if! UNITY_EDITOR
        _textToSpeech = TextToSpeech.Create(OnFinish, OnError);
#endif

    }

    public void Initialise(string text,bool isButton = false)
    {
        mtext = text;

        mButton.interactable = isButton;
    }

    private void OnDisable()
    {
        mButton?.onClick.RemoveAllListeners();
    }


    public async void Speak()
    {

#if !UNITY_EDITOR
        if(isPlaying)
        {
            Stop();
            return;
        }

         if (FinishEvent == null)
            FinishEvent = new UnityEvent();

        if( mtext == null ||  String.IsNullOrWhiteSpace(mtext))
            return;

        string langCode = LanguageController.Instance.GetLangVoiceCode(AppData.mLanguage);
        Debug.Log($"<color=magenta> Speaking..... {langCode} </color>");

        if (mtext.Length > 500)
        {
            string[] versesArray = mtext.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            List<string> versesList = new List<string>(versesArray);

           
            for (int i = 0; i < versesList.Count; ++i)
            {
                Debug.Log("Next line speaking....");

                animator.enabled = true;
                animator.speed = 1.0f;

                isPlaying = true;

                string filteredVal = versesList[i];

                //if (AppData.mLanguage != Language.Chinese ||
                //    AppData.mLanguage != Language.Korean ||
                //    AppData.mLanguage != Language.Hindi)
                //{
                //    filteredVal = Regex.Replace(versesList[i], @"[^a-zA-Z\s]", "");

                //    filteredVal = Regex.Replace(filteredVal, @"\s+", " ");
                //    filteredVal = filteredVal.Trim();
                //}

                _textToSpeech.Speak(filteredVal, langCode, float.Parse("0.8", CultureInfo.InvariantCulture));
                await Spoke(FinishEvent);

                Debug.Log("Going to next line....");
            }

            mButton.image.sprite = mInitialSprite;
            isPlaying = false;
            return;
        }

        //string filtered = Regex.Replace(mtext, @"[^a-zA-Z\s]", "");

        //filtered = Regex.Replace(filtered, @"\s+", " ");
        //filtered = filtered.Trim();

         animator.enabled = true;
        animator.speed = 1.0f;

         isPlaying = true;

        _textToSpeech.Speak(mtext, langCode , float.Parse("0.8", CultureInfo.InvariantCulture));
        await Spoke(FinishEvent);

        if(mButton )
          mButton.image.sprite = mInitialSprite;
#endif

    }

    public Task Spoke(UnityEvent unityEvent)
    {
        var tcs = new TaskCompletionSource<bool>();

        UnityAction handler = null;
        handler = () =>
        {
            tcs.TrySetResult(true);          // Mark the task as completed
            unityEvent.RemoveListener(handler); // Remove listener after invoked
        };

        unityEvent.AddListener(handler);

        return tcs.Task;
    }

    private void OnFinish()
    {
        FinishEvent?.Invoke();
        ResetAction();
        Debug.Log("Speech is finished ..................");
    }

    private void OnError(string msg)
    {
        Debug.LogError($"Error in speech: " + msg);
    }

    public void Stop()
    {
        ResetAction();

#if !UNITY_EDITOR
        _textToSpeech.Stop();

#if UNITY_IOS
          _textToSpeech =  TextToSpeech.Create(OnFinish,OnError);
#endif

#endif

    }

    void ResetAction()
    {
        if (mButton != null)
            mButton.image.sprite = mInitialSprite;

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
