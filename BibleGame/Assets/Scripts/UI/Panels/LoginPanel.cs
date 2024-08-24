using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using BibleGame.Data;
using TMPro;
using System.Text.RegularExpressions;
using BibleGame;


public class LoginPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] Button loginButton;
    [SerializeField] Button hideButton;
    [SerializeField] Button signButton;
    [SerializeField] Button forgetButton;

    [SerializeField] Sprite closeEye;
    [SerializeField] Sprite openEye;

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


    private void LoginAction()
    {
        var validEmail = ValidateEmail(email_Inputfield.text);
        var validPassword = ValidatePassword(password_Inputfield.Text); 
        
        if(!validEmail)
        {
            Debug.LogError("Enter the valid email");
            return;
        }

        if (!validPassword)
        {
            Debug.LogError(error_message);
            return;
        }
    }
}
