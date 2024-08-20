using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using BibleGame;
using TMPro;


public class LoginPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] Button loginButton;
    [SerializeField] Button hideButton;

    [SerializeField] Sprite closeEye;
    [SerializeField] Sprite openEye;

    [SerializeField] TMP_InputField password_Inputfield;


    private const string matchEmailPattern =
      @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
      + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
      + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
      + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

    /// <summary>
    /// Action implemented one enable
    /// </summary>
    private void OnEnable()
    {
        hideButton.onClick.AddListener(HideAction);
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        loginButton.onClick.RemoveAllListeners();
    }


    public void HideAction()
    {
        password_Inputfield.contentType = password_Inputfield.contentType == TMP_InputField.ContentType.Standard ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
        password_Inputfield.ForceLabelUpdate();

        hideButton.image.sprite = password_Inputfield.contentType == TMP_InputField.ContentType.Standard ? openEye : closeEye;
    } 
}
