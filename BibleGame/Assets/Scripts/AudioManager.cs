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
            bgVol = -1.0f;
        }
    }

    public void  PlayButton()
    {
        if (audioSource == null || audioData == null)
            return;

        if (sfxVol >= 0.0f)
            audioSource.volume = sfxVol;

        audioSource.PlayOneShot(audioData.GetAudioClip("Button"), 1.0f);
    }

    public void PlaySFX(SFXType type)
    {
        if (audioSource == null || audioData == null)
            return;

        if (sfxVol >= 0.0f)
            audioSource.volume = sfxVol;

        switch(type)
        {
            case SFXType.correct:
                audioSource.PlayOneShot(audioData.GetAudioClip("Win"), 1.0f);
                break;

            case SFXType.wrong:
                audioSource.PlayOneShot(audioData.GetAudioClip("Wrong"), 1.0f);
                break;

            case SFXType.success:
                audioSource.PlayOneShot(audioData.GetAudioClip("Success"), 1.0f);
                break;
        }
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

        audioSource.volume = bgVol;
    }

    public void EnableVol(bool enable)
    {
        if (enable)
            audioSource.Play();
        else
            audioSource.Pause();
    }
}

public enum SFXType { wrong ,correct,success}
