using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] List<GameObject> stars = new List<GameObject>();

    bool isSelected = false;

    ILevelObj callback;

    private void OnEnable()
    {
        isSelected = false;
        for (int i = 0; i < stars.Count; i++) stars[i].SetActive(false);
    }

    private void OnDisable()
    {
        
    }

    public  void Intialise(string chapterId, ILevelObj callbackIN)
    {
        mChapterId = string.Empty;
        mChapterId = chapterId;

        callback = callbackIN;
    }

    public void StarUpdate(int starCount)
    {
        if (starCount > stars.Count)
        {
            Debug.LogError("Stars are greater than 3");
            return;
        }

        for (int i = 0; i < starCount; i++) stars[i].SetActive(true);
    }

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
}
