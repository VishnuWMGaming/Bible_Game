using BibleGame.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BibleGame.Data;
using static BibleGame.API.GetBiblesAPI;
using BibleGame;

public class BiblePanel : MonoBehaviour,IBible
{
    [Header("Bible Settings:")]
    [SerializeField] Bible mBibleOne;
    [SerializeField] Bible mBibleTwo;

    private void OnEnable()
    {
        Intialise();
    }

    private void OnDisable()
    {
        
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

    public void BibleSelect(string id)
    {
        if (string.IsNullOrEmpty(id))
            return;

        UserData.bibleId = id;
        Actions.ChangePanelActions(CanvasType.testament);
    }

}
