using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VideoData", menuName = "BibleGameStore/VideoData")]
public class VideoUrlData : ScriptableObject
{
    [Header("VideoUrls:")]
    [TextArea(2,4)]
    [SerializeField] List<string> videoUrls = new List<string>();


    public List<string> GetAll()
    {
        return videoUrls;   
    }
}