using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(TMP_Text))]
public class PagePanel : MonoBehaviour
{
    [SerializeField] Page page;
    public Page Page => page;

    TMP_Text pageText;

    private void OnEnable()
    {
        pageText = this.GetComponent<TMP_Text>();
    }
}

[Serializable]
public class Page
{
    public int chapterIndex;
    public int pageIndex;

    public Page(int chapterIndex, int pageIndex)
    {
        this.chapterIndex = chapterIndex;
        this.pageIndex = pageIndex;
    }
}

