using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IBible
{
    public void BibleSelect(string id,string name);
}

[RequireComponent(typeof(Button))]
public class Bible : MonoBehaviour
{
    Button mButton;
    IBible callback;

    [SerializeField] TMP_Text mName;

    [SerializeField] TranslateLang translateLang;

    string id;
    public string mID => id;

    private void OnEnable()
    {
        mButton = GetComponent<Button>();
        mButton?.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); callback.BibleSelect(id,mName.text); });
    }

    private void OnDisable()
    {
        mButton?.onClick.RemoveAllListeners();
    }

    public void Intialise(string _id,string name, IBible callbackIN)
    {
        id = _id;
        mName.text = name;

        translateLang.UpdateValue(name);
        translateLang.UpdateText(() => { });

        callback = callbackIN;
    }
}
