using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using static BibleGame.API.GetBiblesAPI;

public class BiblePanel : MonoBehaviour,IBible
{
    [Header("Bible Settings:")]
    [SerializeField] Bible mBibleOne;
    [SerializeField] Bible mBibleTwo;

    [SerializeField] Button mBackButton;


    private void OnEnable()
    {
        mBackButton?.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.ageSelect); });
        Intialise();
    }

    private void OnDisable()
    {
        mBackButton?.onClick.RemoveAllListeners();
    }
    
    void Intialise()
    {
        BibleData bibleDataOne = GetBiblesAPI.mBibleDatas[0];

        PopUp.Instance.EnableLoad(true);
        GetBiblesAPI.GetDetail((success, res) =>
        {
            PopUp.Instance.EnableLoad(false);

            if (!success)
            {
                Debug.LogError("Error in getting bible details");
                return;
            }

            mBibleOne.Intialise(bibleDataOne.bible_id, bibleDataOne.name, this);

        }, bibleDataOne.bible_id);


        BibleData bibleDataTwo = GetBiblesAPI.mBibleDatas[1];

        PopUp.Instance.EnableLoad(true);
        GetBiblesAPI.GetDetail((success, res) =>
        {
            PopUp.Instance.EnableLoad(false);

            if (!success)
            {
                Debug.LogError("Error in getting bible details");
                return;
            }

            mBibleTwo.Intialise(bibleDataTwo.bible_id, bibleDataTwo.name, this);

        }, bibleDataTwo.bible_id);

    }

    public void BibleSelect(string id,string name)
    {
        if (string.IsNullOrEmpty(id))
            return;

        Debug.Log($"<color= green> Bible:{id} {name} is selected </color>");

        UserData.bibleName = name ;
        UserData.bibleId = id;
        Actions.ChangePanelActions(CanvasType.testament);
    }

}
