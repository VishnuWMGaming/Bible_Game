using BibleGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestamentPanel : MonoBehaviour
{ 
    public void OldTestamentAction() 
    {
        GameData.testament = Testament.Old;

        Actions.ChangePanelActions(CanvasType.level);
    }

    public void NewTestamentAction()
    {
        GameData.testament = Testament.New;
        Actions.ChangePanelActions(CanvasType.level);
    }
}

public enum Testament
{
    Old,New
}
