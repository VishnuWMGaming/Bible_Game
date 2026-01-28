using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class Version : MonoBehaviour
{
    TMP_Text mText;

    private void Awake()
    {
        mText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        mText.text = $"ver {Application.version}";
    }
}
