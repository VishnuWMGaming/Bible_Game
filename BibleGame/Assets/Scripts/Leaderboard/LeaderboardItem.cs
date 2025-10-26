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
    public void Initialize(string name, int rank,Sprite sprite)
    {
        nameTxt.text = name;
        rankTxt.text = rank.ToString();

        if(sprite != null)
         profileImg.SetRightSize(sprite, true);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
