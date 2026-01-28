using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "BibleGameStore/AudioData")]
public class AudioData : ScriptableObject
{
    [Header("AudioClips:")]
    [SerializeField] List<AudioD> audios = new List<AudioD>();

    public AudioClip GetAudioClip(AudioType type)
    {
        return audios.Find(x => x.type == type).clip;
    }
}

public enum AudioType {button,Win,Correct,Wrong,Bg,teenager,reading,kindergarden,elementary,adult}

[Serializable]
public class AudioD
{
    public AudioClip clip;
    public string name;
    public AudioType type;
}
