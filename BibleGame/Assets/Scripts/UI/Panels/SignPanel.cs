using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

using BibleGame.API;
using BibleGame;

using System.Text.RegularExpressions;
using BibleGame.Data;

public class SignPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] TMP_InputField name_InputField;
    [SerializeField] TMP_InputField email_InputField;
    [SerializeField] UIPasswordField password_InputField;
    [SerializeField] UIPasswordField password_InputFieldConfirm;

    [SerializeField] Button loginButton;
    [SerializeField] Button signUpButton;


   private const string matchEmailPattern =
      @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
      + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
      + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
      + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";


    string error_message;



    private void OnEnable()
    {
        email_InputField.text = "";
        name_InputField.text = "";

        loginButton.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.login));
        signUpButton.onClick.AddListener(SignUpAction);
    }

    private void OnDisable()
    {
        loginButton.onClick.RemoveAllListeners();
        signUpButton.onClick.RemoveAllListeners();
    }

    
    /// <summary>
    /// Action implemented sign up is clicked
    /// </summary>
    private void SignUpAction()
    {
        var successMail = ValidateEmail(email_InputField.text);
        var successPassword = ValidatePassword(password_InputField.Text, password_InputFieldConfirm.Text);
        var validate = Validate();

        if (!validate)
        {
            Debug.LogError("Fields cannot be empty");
            PopUp.Instance.ShowMessage("Fields cannot be empty");
            return;
        }

        if (!successMail)
        {
            Debug.LogError("Not the valid mail");
            PopUp.Instance.ShowMessage("Not the valid mail");
            return;
        }

        if(!successPassword)
        {
            Debug.LogError(error_message);
            PopUp.Instance.ShowMessage(error_message);
            return;
        }

        PopUp.Instance.EnableLoad(true);
        var registerData  = new RegisterData(name_InputField.text,email_InputField.text,password_InputFieldConfirm.Text);

        RegisterAPI.RegisterUser(registerData, RegisterCallback);
    }

    private void RegisterCallback(bool success, RegisterResponse response)
    {
        PopUp.Instance.EnableLoad(false);

        if (success && response.succeeded)
        {
            Debug.Log("Response data >>>" + response.ResponseData);
            AppData.otpData = new OTPData(response.ResponseData.otp,OTPType.sign);
            AppData.loginData = new LoginData(email_InputField.text, password_InputFieldConfirm.Text, name_InputField.text);

            Actions.ChangePanelActions(CanvasType.otp);
        }
        else
        {
            Debug.LogError("Response failed !!!" + response != null ? response.ResponseMessage : "Network issue");
            PopUp.Instance.SendMessage(response != null ? response.ResponseMessage : "Network issue");
        }
    }


    private static bool ValidateEmail(string email)
    {
        if (email != null)
            return Regex.IsMatch(email, matchEmailPattern);
        else
            return false;
    }

    private bool ValidatePassword(string password , string confirmPassword)
    {
        bool isValid = true;

        if (password == null || password == string.Empty)
        {
            error_message = "Please enter password";
            isValid = false;
        }
        else if (password.Length < 8)
        {
            error_message = "Password length must be 8 character";
            isValid = false;
        }
        else if(password != confirmPassword)
        {
            error_message = "Password does not match";
            isValid = false;
        }

        return isValid;
    }

    private bool Validate()
    {
        return !string.IsNullOrEmpty(email_InputField.text)
               && !string.IsNullOrEmpty(password_InputField.Text)
               && !string.IsNullOrEmpty(password_InputFieldConfirm.Text)
               && !string.IsNullOrEmpty(name_InputField.text);
        //&& !string.IsNullOrEmpty(cityName) && !string.IsNullOrEmpty(mobileNumber) && !string.IsNullOrEmpty(age) && !string.IsNullOrEmpty(lastName);
    }
}
