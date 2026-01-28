using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IMBook
{
    public void SelectAction(string id);
}

[RequireComponent(typeof(Button))]
public class MBook : MonoBehaviour
{
    Button mButton;

    [SerializeField] TMP_Text mTitle;
    [SerializeField] TMP_Text mSubTitle;

    [SerializeField] TranslateLang mTranslateT;
    [SerializeField] TranslateLang mTranslateS;

    string mBibleId;
    public string BibleId => mBibleId;

    string mid;
    public string ID => mid;

    IMBook callback;

    private void OnEnable()
    {
        mButton = GetComponent<Button>();
        mButton?.onClick.AddListener(() =>
        {
            if (callback == null)
                return;

            callback.SelectAction(mid);
        });
    }

    private void OnDisable()
    {
        
    }

    public void Intialise(BibleGame.API.GetBiblesAPI.BookData data , IMBook callbackIN)
    {
        mTitle.text = data.name;
        mSubTitle.text = data.nameLong;

        mTranslateT.UpdateValue(data.name);
        mTranslateT.UpdateText(() => { });

        mTranslateS.UpdateValue(data.name);
        mTranslateS.UpdateText(() => { });

        mid = data.id;
        mBibleId = data.bibleId;

        callback = callbackIN;
    }
}
