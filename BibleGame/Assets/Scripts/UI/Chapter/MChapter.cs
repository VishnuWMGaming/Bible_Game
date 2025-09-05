using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using BibleGame.API;
using BibleGame.Data;

public interface IMChapter
{
    public void SelectAction(string id);
}

[RequireComponent(typeof(Button))]
public class MChapter : MonoBehaviour
{

    Button mButton;

    [SerializeField] TMP_Text mTitle;
    [SerializeField] TMP_Text mSubTitle;

    string mBibleId;
    public string BibleId => mBibleId;

    string mid;
    public string ID => mid;

    IMChapter callback;

    private void OnEnable()
    {
        mButton = GetComponent<Button>();
        mButton?.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayButton();

            if (callback == null)
                return;

            callback.SelectAction(mid);
        });
    }

    private void OnDisable()
    {
        mButton?.onClick.RemoveAllListeners();
    }

    public void Intialise(ChapterData data, IMChapter callbackIN)
    {
        mTitle.text = $"Chapter {data.number}";
        mSubTitle.text = "";

        mid = data.id;

        callback = callbackIN;
    }
}
