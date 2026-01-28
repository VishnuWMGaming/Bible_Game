using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public interface ILevelObj
{
    public void ChapterSelect(string id);
}

public class LevelObj : MonoBehaviour
{
    [SerializeField] string mChapterId;
    public string ID => mChapterId;

    [SerializeField] GameObject correctMark;

    [Space]
    [SerializeField] TMP_Text chapterID;

    bool isSelected = false;

    ILevelObj callback;

    private void OnEnable()
    {
        isSelected = false;
        correctMark.SetActive(false);
    }

    private void OnDisable()
    {

    }

    public void Intialise(string chapterId, ILevelObj callbackIN)
    {
        mChapterId = string.Empty;
        mChapterId = chapterId;

       if(chapterID  != null )
        chapterID.text = mChapterId;

        callback = callbackIN;
    }

    public void Finish(bool enable) { correctMark.SetActive(enable); }

    private void OnMouseDown()
    {
        if (isSelected)
            return;

        if (String.IsNullOrEmpty(mChapterId))
            return;

        isSelected = true;
        Debug.Log($"Chapter is selected {mChapterId}");

        callback.ChapterSelect(mChapterId);
    }

    public void Clear()
    {
        mChapterId = string.Empty;
        callback = null;
    }
}
