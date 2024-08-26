using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class UIPasswordField : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] Button hideButton;

    [SerializeField] Sprite closeEye;
    [SerializeField] Sprite openEye;

    TMP_InputField inputField;

    public string Text => inputField.text;

    private void OnEnable()
    {
       inputField = GetComponent<TMP_InputField>();
       hideButton.onClick.AddListener(HideAction);

        inputField.text = "";
    }

    public void HideAction()
    {
        inputField.contentType = inputField.contentType == TMP_InputField.ContentType.Standard ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
        inputField.ForceLabelUpdate();

        hideButton.image.sprite = inputField.contentType == TMP_InputField.ContentType.Standard ? openEye : closeEye;
    }
}
