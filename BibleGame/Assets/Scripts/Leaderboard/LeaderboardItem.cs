using DebugUtils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItem : MonoBehaviour, ICell
{
    public PostImage profileImg;
    public TMP_Text nameTxt;
    public TMP_Text rankTxt;
    public void Initialize(string name, int rank, Sprite sprite)
    {
        nameTxt.text = name;
        rankTxt.text = rank.ToString();

        profileImg.Reset();

        if (sprite != null)
        {
           // DevDebug.Log($"Adding the sprite ... {name}", DebugColor.Magenta);

            profileImg.SetRightSize(sprite, true);
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
