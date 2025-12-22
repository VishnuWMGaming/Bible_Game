using NUnit.Framework.Interfaces;
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
            audioSource.volume = bgVol >=  1.0f ? maxVolBg : bgVol;
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

        if (sfxVol >= 0.0f)
            audioSource.volume = sfxVol;

        audioSource.PlayOneShot(audioData.GetAudioClip(AudioType.button), 1.0f);
    }

    public void PlaySFX(AudioType type)
    {
        if (audioSource == null || audioData == null)
            return;

        if (sfxVol >= 0.0f)
            audioSource.volume = sfxVol;

        audioSource.PlayOneShot(audioData.GetAudioClip(type), 1.0f);
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

        float vol = type == AudioType.reading ? 
                                                bgVol <= maxVolReadBg ? bgVol : maxVolReadBg
                                                : bgVol >= 1.0f ? maxVolBg : bgVol; 

        audioSource.volume = vol;
                                                
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void ChangeVolSfx(float vol)
    {
        sfxVol = vol;
        PlayerPrefs.SetFloat("SfxVol", sfxVol);
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

        audioSource.volume = bgVol >= 1.0f ? maxVolBg : bgVol;
    }
}

public enum SFXType { wrong ,correct,success}
