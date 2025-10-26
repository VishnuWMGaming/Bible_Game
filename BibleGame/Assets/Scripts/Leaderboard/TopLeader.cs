using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TopLeader : MonoBehaviour
{
    [SerializeField] PostImage pic;
    [SerializeField] TMP_Text mScore;
    [SerializeField] TMP_Text mName;


    public void Init(string name, float score, Sprite sprite)
    {
        mName.text = name;
        mScore.text = score.ToString("F2");

        if (sprite != null)
        pic.SetRightSize(sprite, true);
    }
}
