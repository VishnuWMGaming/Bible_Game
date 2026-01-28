using BibleGame;
using BibleGame.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Chapters : MonoBehaviour,IMChapter
{
    [SerializeField] private Button backBtn;

    [Space]
    [SerializeField] GameObject mBookObj;
    [SerializeField] Transform mBookTransform;

    private void OnEnable()
    {
        backBtn.onClick.AddListener(() => Actions.StartPageAction(StartPage.book));

        Initialise();
    }

    private void OnDisable()
    {
        backBtn.onClick.RemoveAllListeners();

        ClearAll();
    }

    public void SelectAction(string id)
    {
        if (String.IsNullOrEmpty(id))
            return;

        UserData.chapterId = id;

        Actions.StartPageAction(StartPage.chapter);
    }

    void Initialise()
    {
        if (GameData.mChapterDatas == null || GameData.mChapterDatas.Count == 0)
            return;

        foreach(var chapter in GameData.mChapterDatas)
        {
            GameObject chapterObj = Instantiate(mBookObj, mBookTransform);

            chapterObj.transform.localScale = Vector3.one;

            MChapter mChapter = chapterObj.GetComponent<MChapter>();
            mChapter.Intialise(chapter, this);
        }
    }


    public void ClearAll()
    {
        for (int i = 0; i < mBookTransform.childCount; i++)
        {
            GameObject go = mBookTransform.GetChild(i).gameObject;
            Destroy(go);
        }
    }
}
