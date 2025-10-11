using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "BibleGameStore/AudioData")]
public class AudioData : ScriptableObject
{
    [Header("AudioClips:")]
    [SerializeField] List<AudioD> audios = new List<AudioD>();

    public AudioClip GetAudioClip(string audioname)
    {
        return audios.Find(x => x.name == audioname).clip;
    }
}

public enum AudioType {sfx,Bg}

[Serializable]
public class AudioD
{
    public AudioClip clip;
    public string name;
    public AudioType type;
}
