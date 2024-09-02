using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BibleGame;
using TMPro;
using BibleGame.Data;


public interface ICover
{
    public void SelectChapter(string chapter);
}

public class Cover : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] Button homeBtn;
    [SerializeField] Button chapterBtn;
    [SerializeField] TMP_Text _username;

    public ICover caklback;

    private void OnEnable()
    {
        _username.text = AppData.loginData.Name;

        homeBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.home));
        chapterBtn.onClick.AddListener(() => caklback.SelectChapter("Jesus"));
    }

    private void OnDisable()
    {
       homeBtn.onClick.RemoveAllListeners();
       chapterBtn.onClick.RemoveAllListeners();    
    }

}
