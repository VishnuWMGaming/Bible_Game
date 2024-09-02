using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using BibleGame;

public class LevelPanel : MonoBehaviour,ILevelButton ,ICover
{
    [Header("UI Settings:")]
    [SerializeField] Button settingsBtn;
    [SerializeField] Button backBtn;


    [Header("Level Buttons:")]
    [SerializeField] List<Level_Button> level_Buttons = new List<Level_Button>();


    private void OnEnable()
    {
        for (int i = 0; i < level_Buttons.Count; i++)
            level_Buttons[i].callback = this;

        settingsBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.setPanel));
        backBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.home));
    }

    private void OnDisable()
    {
        settingsBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();   
    }


    /// <summary>
    /// Action implemented on level selection
    /// </summary>
    /// <param name="level"></param>
    public void OnLevelSelectAction(int level)
    {
        Actions.ChangePanelActions(CanvasType.chapter);
    }

    /// <summary>
    /// Action implemented on select chapter
    /// </summary>
    /// <param name="chapter"></param>
    public void SelectChapter(string chapter)
    {
        
    }
}
