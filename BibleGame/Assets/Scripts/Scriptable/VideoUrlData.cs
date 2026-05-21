using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VideoData", menuName = "BibleGameStore/VideoData")]
public class VideoUrlData : ScriptableObject
{
    [Header("VideoUrls:")]
    [SerializeField] List<VidData> videoUrls = new List<VidData>();


    public List<VidData> GetAll()
    {
        return videoUrls;   
    }
}

[Serializable]
public class VidData
{
    [TextArea(2, 4)]
    public string youtubeLink;
    [TextArea(2, 4)]
    public string vidStoreLink;
}