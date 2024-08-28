using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using BibleGame.Data;
using TMPro;
using System.Text.RegularExpressions;

using BibleGame;
using BibleGame.API;
using System;
using Unity.VisualScripting;

public class LoginPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] Button loginButton;
    [SerializeField] Button hideButton;
    [SerializeField] Button signButton;
    [SerializeField] Button forgetButton;

    [SerializeField] UIPasswordField password_Inputfield;
    [SerializeField] TMP_InputField email_Inputfield;


    string error_message;

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
        loginButton.onClick.AddListener(LoginAction);
        signButton.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.signup));
        forgetButton.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.forgetpassword));

        email_Inputfield.text = "";
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        loginButton.onClick.RemoveAllListeners();
        signButton.onClick.RemoveAllListeners();
        forgetButton.onClick.RemoveAllListeners();  
    }

    private static bool ValidateEmail(string email)
    {
        if (email != null)
            return Regex.IsMatch(email, matchEmailPattern);
        else
            return false;
    }


    private  bool ValidatePassword(string password)
    {
        bool isValid = false;   

        if(password == null || password == string.Empty)
        {
            error_message = "Please enter the password";
            isValid = false;
        }
        else if(password.Length < 8)
        {
            error_message = "Password length must be 8 character";
        }
        else
            isValid = true;

        return isValid; 
    }

    private bool Validate()
    {
        return !string.IsNullOrEmpty(email_Inputfield.text)
               && !string.IsNullOrEmpty(password_Inputfield.Text);
        //&& !string.IsNullOrEmpty(cityName) && !string.IsNullOrEmpty(mobileNumber) && !string.IsNullOrEmpty(age) && !string.IsNullOrEmpty(lastName);
    }


    private void LoginAction()
    {
        var validEmail = ValidateEmail(email_Inputfield.text);
        var validPassword = ValidatePassword(password_Inputfield.Text);
        var valid = Validate();

        if (!valid)
        {
            Debug.LogError("Please enter Email & Password");
            PopUp.Instance.ShowMessage("Please enter Email & Password");
            return;
        }

        if (!validEmail)
        {
            Debug.LogError("Enter the valid email");
            PopUp.Instance.ShowMessage("Enter the valid email");
            return;
        }

        if (!validPassword)
        {
            Debug.LogError(error_message);
            PopUp.Instance.ShowMessage(error_message);
            return;
        }

        PopUp.Instance.EnableLoad(true);

        var loginRequestData = new LoginRequest(email_Inputfield.text, password_Inputfield.Text);
        LoginAPI.Login(loginRequestData, Callback);
    }

    private void Callback(bool success, LoginResponseX response)
    {
        PopUp.Instance.EnableLoad(false);

        if (success)
        {
            AppData.loginData = new LoginData(email_Inputfield.text, password_Inputfield.Text, response.ResponseData.name);
            Actions.ChangePanelActions(CanvasType.home);
        }
        else
        {
            PopUp.Instance.ShowMessage(response.ResponseMessage);
        }
    }
}
