using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;
    public static AudioManager Instance { get { return instance; } }

    [SerializeField] AudioData audioData;

    [SerializeField] AudioSource audioSource;

    float sfxVol = 0.0f;
   

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
    }

    public void  PlayButton()
    {
        if (audioSource == null || audioData == null)
            return;

        if (sfxVol >= 0.0f)
            audioSource.volume = sfxVol;

        audioSource.PlayOneShot(audioData.GetAudioClip("Button"), 1.0f);
    }

    public void ChangeVolSfx(float vol)
    {
        sfxVol = vol;
        PlayerPrefs.SetFloat("SfxVol", sfxVol);
    }
}
