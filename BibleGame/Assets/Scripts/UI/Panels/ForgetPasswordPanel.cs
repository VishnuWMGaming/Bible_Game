using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using BibleGame.API;
using System.Text.RegularExpressions;
using System;
using BibleGame.Data;
using BibleGame;

public class ForgetPasswordPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button _nextBtn;
    [SerializeField] Button _backBtn;

    private const string matchEmailPattern =
      @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
      + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
      + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
      + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

    /// <summary>
    /// Action implemented on enable
    /// </summary>
    private void OnEnable()
    {
        _nextBtn.onClick.AddListener(ForgetPasswordAction);
        _backBtn.onClick.AddListener(()=> Actions.ChangePanelActions(CanvasType.login));

        inputField.text = "";
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        _nextBtn.onClick.RemoveAllListeners();
        _backBtn.onClick.RemoveAllListeners();
    }


    private static bool ValidateEmail(string email)
    {
        if (email != null)
            return Regex.IsMatch(email, matchEmailPattern);
        else
            return false;
    }

    void ForgetPasswordAction()
    {
        if(String.IsNullOrEmpty(inputField.text))
        {
            PopUp.Instance.ShowMessage("Please enter email");
            return;
        }

        var successmail = ValidateEmail(inputField.text);

        if(!successmail)
        {
            PopUp.Instance.ShowMessage("Invalid Email");
            return;
        }

        PopUp.Instance.EnableLoad(true);

        var emailData =  new EmailData(inputField.text);
        ForgetAPI.Retrive(emailData, Callback);
    }

    private void Callback(bool success, ForgetResponse response)
    {
        PopUp.Instance.EnableLoad(false);

        if (success)
        {
            AppData.otpData = new OTPData(response.ResponseData.otp, OTPType.forget);

            Actions.ChangePanelActions(CanvasType.otp);
        }
        else
        {
            PopUp.Instance.ShowMessage(response.ResponseMessage);
        }
    
    }
}
