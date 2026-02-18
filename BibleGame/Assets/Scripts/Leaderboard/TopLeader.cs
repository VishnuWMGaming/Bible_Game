using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface ITopLeader
{
    public void ShowInfo(string id);
}

public class TopLeader : MonoBehaviour
{
    [SerializeField] PostImage pic;
    [SerializeField] TMP_Text mScore;
    [SerializeField] TMP_Text mName;

    [SerializeField] Button mShowBtn;

    [Space]
    [SerializeField] string mUserID;

    ITopLeader callback;


    private void OnEnable()
    {
      
    }

    private void OnDisable()
    {
        mShowBtn?.onClick.RemoveAllListeners();
    }


    public void Init(string id, string name, float score, Sprite sprite,ITopLeader callbackIN)
    {
        mName.text = name;
        mScore.text = "";

        mUserID = id;

        callback = callbackIN;

        mShowBtn?.onClick.AddListener(() =>
        {
            callback?.ShowInfo(mUserID);
        });

        if (sprite != null)
        pic.SetRightSize(sprite, true);
    }
}
