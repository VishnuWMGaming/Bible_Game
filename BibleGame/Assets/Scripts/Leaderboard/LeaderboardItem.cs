using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItem : MonoBehaviour, ICell
{
    public Image profileImg;
    public TMP_Text nameTxt;
    public TMP_Text rankTxt;
    public void Initialize(string name, int rank)
    {
        nameTxt.text = name;
        rankTxt.text = rank + "";
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
