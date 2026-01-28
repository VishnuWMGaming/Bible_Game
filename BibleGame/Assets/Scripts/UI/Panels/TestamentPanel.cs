using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestamentPanel : MonoBehaviour
{
    [SerializeField] Button mBackButton;


    private void OnEnable()
    {
        mBackButton?.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.bible); });
    }

    private void OnDisable()
    {
        mBackButton?.onClick.RemoveAllListeners();
    }


    public void OldTestamentAction() 
    {
        UserData.testament = Testament.Old;
        //  Actions.ChangePanelActions(CanvasType.level);

       // AppData.mCurrentPage = StartPage.book;
        Actions.ChangePanelActions(CanvasType.chapter);
        Actions.StartPageAction(StartPage.book);
    }

    public void NewTestamentAction()
    {
        UserData.testament = Testament.New;
        // Actions.ChangePanelActions(CanvasType.level);

        //AppData.mCurrentPage = StartPage.book;
        Actions.ChangePanelActions(CanvasType.chapter);
        Actions.StartPageAction(StartPage.book);
    }

    
}

public enum Testament
{
    Old,New
}
