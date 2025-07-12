using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestamentPanel : MonoBehaviour
{ 
    public void OldTestamentAction() 
    {
        UserData.testament = Testament.Old;
        //  Actions.ChangePanelActions(CanvasType.level);

        AppData.mCurrentPage = StartPage.book;
        Actions.ChangePanelActions(CanvasType.chapter);
    }

    public void NewTestamentAction()
    {
        UserData.testament = Testament.New;
        // Actions.ChangePanelActions(CanvasType.level);

        AppData.mCurrentPage = StartPage.book;
        Actions.ChangePanelActions(CanvasType.chapter);
    }

    
}

public enum Testament
{
    Old,New
}
