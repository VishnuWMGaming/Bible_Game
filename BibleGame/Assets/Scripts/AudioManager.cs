using BibleGame.API;
using BibleGame.Data;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;
    public static AudioManager Instance { get { return instance; } }

    [SerializeField] AudioData audioData;

    [SerializeField] AudioSource audioSource;

    [SerializeField] AudioSource voiceAudioSource;

    float sfxVol = 0.0f;
    float bgVol = 0.0f;

    [Header("Settings:")]
    [Range(0,1.0f)]
    [SerializeField] float maxVolBg;
    [Range(0, 1.0f)]
    [SerializeField] float maxVolReadBg;

    private void Awake()
    {
        if(instance == null)
            instance = this;

        if (PlayerPrefs.HasKey("SfxVol"))
        {
            sfxVol = PlayerPrefs.GetFloat("SfxVol");
        }
        else
            sfxVol = -1.0f;

        if (PlayerPrefs.HasKey("BGVol"))
        {
            bgVol = PlayerPrefs.GetFloat("BGVol");
            audioSource.volume = bgVol;
            EnableVol(true);
        }
        else
        {
            EnableVol(true);
            bgVol = 1.0f;

            ChangeVolBG(bgVol);
        }
    }

    public void  PlayButton()
    {
        if (audioSource == null || audioData == null)
            return;

        //if (sfxVol >= 0.0f)
        //    audioSource.volume = sfxVol;

        audioSource.PlayOneShot(audioData.GetAudioClip(AudioType.button), sfxVol);
    }

    public void PlaySFX(AudioType type)
    {
        if (audioSource == null || audioData == null)
            return;

        audioSource.PlayOneShot(audioData.GetAudioClip(type), sfxVol);
    }

    public void PlayBG(AudioType type)
    {
        AudioClip clip = audioData.GetAudioClip(type);

        if(clip == null)
        {
            Debug.LogError($"{type} BG not found");
            return;
        }

        if (audioSource.clip == clip)
            return;

        bgVol = PlayerPrefs.GetFloat("BGVol");

        float vol = bgVol;

        audioSource.volume = vol;
                                                
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void ChangeVolSfx(float vol)
    {
        sfxVol = vol;
        PlayerPrefs.SetFloat("SfxVol", sfxVol);
    }

    public void PlayVoice(string text,Action onComplete = null)
    {
        StopAllCoroutines();

        VoiceOverAPI.Get(this, (success, clip) =>
        {
            if(!success)
            {
                onComplete?.Invoke();
                return;
            }

            voiceAudioSource.clip = clip;
            voiceAudioSource.Play();

            voiceAudioSource.volume = 1.0f;

            StartCoroutine(WaitForVoiceEnd(clip.length, onComplete));

        },text,AppData.mLanguage);
    }

    public void EndVoice()
    {
        voiceAudioSource.Stop();
    }

    private IEnumerator WaitForVoiceEnd(float duration, Action onComplete)
    {
        yield return new WaitForSeconds(duration);

        onComplete?.Invoke();
    }


    public void ChangeVolBG(float vol)
    {
        bgVol = vol;
        PlayerPrefs.SetFloat("BGVol", bgVol);

        audioSource.volume = bgVol >= 1.0f ? maxVolBg : bgVol;
    }

    public void EnableVol(bool enable,AudioType audioType = AudioType.Bg)
    {
        if (enable)
            audioSource.Play();
        else
            audioSource.Pause();

        audioSource.volume = bgVol;
    }


    public void MuteBG(bool enable)
    {
        bgVol = PlayerPrefs.GetFloat("BGVol");

        audioSource.volume = enable? 0 : bgVol; 
    }
}

public enum SFXType { wrong ,correct,success}
