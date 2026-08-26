using System;
using UnityEngine;

using System.Collections.Generic;

[CreateAssetMenu(fileName = "BotChatData", menuName = "BibleGameStore/BotChat")]
public class ChatAssetData : ScriptableObject
{
    [Header("ChatAssets:")]
    [SerializeField] List<BotChatAsset> datas = new List<BotChatAsset>();


    public TextAsset Get(string key)
    {
        return datas.Find(x => x.mName == key).data;
    }
}

[Serializable]
public class BotChatAsset
{
    public string mName;
    public TextAsset data;
}

