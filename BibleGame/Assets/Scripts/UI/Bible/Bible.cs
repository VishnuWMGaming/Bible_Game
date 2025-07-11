using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IBible
{
    public void BibleSelect(string id);
}

[RequireComponent(typeof(Button))]
public class Bible : MonoBehaviour
{
    Button mButton;
    IBible callback;

    [SerializeField] TMP_Text mName;

    string id;
    public string mID => id;

    private void OnEnable()
    {
        mButton = GetComponent<Button>();
        mButton?.onClick.AddListener(() => callback.BibleSelect(id));
    }

    private void OnDisable()
    {
        mButton?.onClick.RemoveAllListeners();
    }

    public void Intialise(string _id,string name, IBible callbackIN)
    {
        id = _id;
        mName.text = name;
        callback = callbackIN;
    }
}
