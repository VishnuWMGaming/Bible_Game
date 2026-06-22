using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CountryLoader;

public class SignPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] TMP_InputField name_InputField;
    [SerializeField] TMP_InputField email_InputField;
    [SerializeField] UIPasswordField password_InputField;
    [SerializeField] UIPasswordField password_InputFieldConfirm;
    [SerializeField] TMP_InputField churchName_InputField;
    [SerializeField] TMP_InputField mobileNumber_InputField;
    [SerializeField] TMP_Text phoneCodeTxt;

    [SerializeField] Button loginButton;
    [SerializeField] Button signUpButton;

    [Space]
    [SerializeField] CountryLoader countryLoader;


    string phoneCode;


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

        loginButton.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.login); });
        signUpButton.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); SignUpAction(); });

        phoneCodeTxt.text = "Phone Code";

        phoneCode = "";
        countryLoader.SetPhoneCode += SetPhoneCode;

        mobileNumber_InputField.onSelect.AddListener((str) =>
        {
            if(string.IsNullOrWhiteSpace(phoneCode))
            {
                PopUp.Instance.ShowMessage("Please select the country code");
                return;
            }
        });
    }

    private void OnDisable()
    {
        loginButton.onClick.RemoveAllListeners();
        signUpButton.onClick.RemoveAllListeners();
        mobileNumber_InputField.onSelect.RemoveAllListeners();

        countryLoader.SetPhoneCode -= SetPhoneCode;
    }

    private void SetPhoneCode(CountryPhoneData phoneData)
    {
        phoneCode = phoneData.phoneCode;
        mobileNumber_InputField.characterLimit = phoneData.digitLength;

        phoneCodeTxt.text = $"+ {phoneData.phoneCode}";
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

            error_message = string.IsNullOrEmpty(password_InputFieldConfirm.Text) ? "Please enter Confirm Password" : string.Empty;
            error_message = string.IsNullOrEmpty(password_InputField.Text) ? "Please enter Password" : error_message;
            error_message = string.IsNullOrEmpty(email_InputField.text) ? "Please enter Email" : error_message;
            error_message = string.IsNullOrEmpty(name_InputField.text) ? "Please enter Name" : error_message;

            error_message = string.IsNullOrEmpty(email_InputField.text)
               && string.IsNullOrEmpty(password_InputField.Text)
               && string.IsNullOrEmpty(password_InputFieldConfirm.Text)
               && string.IsNullOrEmpty(name_InputField.text) ? "Please fill all required fields" : error_message;


            PopUp.Instance.ShowMessage(error_message);
            return;
        }

        if (!successMail)
        {
            Debug.LogError("Not the valid mail");
            PopUp.Instance.ShowMessage("Please enter Valid Email");
            return;
        }

        if(!successPassword)
        {
            Debug.LogError(error_message);
            PopUp.Instance.ShowMessage(error_message);
            return;
        }


        //if(string.IsNullOrWhiteSpace(churchName_InputField.text))
        //{
        //    Debug.LogError(error_message);
        //    PopUp.Instance.ShowMessage("Please enter the church name");
        //    return;
        //}

        string phoneNumber = $"+{phoneCode}{mobileNumber_InputField.text}";

        PopUp.Instance.EnableLoad(true);
        var registerData  = new RegisterData(name_InputField.text,email_InputField.text,password_InputFieldConfirm.Text, churchName_InputField.text, phoneNumber);

        PlayerPrefs.SetString("ChurchName", churchName_InputField.text);

        RegisterAPI.RegisterUser(registerData, RegisterCallback);
    }

    private void RegisterCallback(bool success, RegisterResponse response)
    {
        PopUp.Instance.EnableLoad(false);

        if (success && response.succeeded)
        {
            Debug.Log("Response data >>>" + response.ResponseData);
            AppData.mSignOtpData = new OTPData(response.ResponseData.otp,OTPType.sign);
            AppData.loginData = new LoginData(email_InputField.text, password_InputFieldConfirm.Text, name_InputField.text, churchName_InputField.text);

            AppData.otpPage = OTPType.sign;
            Actions.ChangePanelActions(CanvasType.otp);
        }
        else
        {
            Debug.LogError("Response failed !!!" + response != null ? response.ResponseMessage : "Network issue");
            PopUp.Instance.ShowMessage(response != null ? response.ResponseMessage : "Network issue");
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
        else if(!IsStrongPassword(password))
        {
            error_message = "Password should be between 8 to 16 characters and should include 1 Uppercase, 1 Lowercase, 1 Number and 1 Special Character";
            isValid = false;
        }
        else if(password != confirmPassword)
        {
            error_message = "Password didn't matched";
            isValid = false;
        }

        return isValid;
    }

    public bool IsStrongPassword(string password)
    {
        if (password.Length < 8)
            return false;

        // Check for uppercase letter
        if (!System.Text.RegularExpressions.Regex.IsMatch(password, "[A-Z]"))
            return false;

        // Check for lowercase letter
        if (!System.Text.RegularExpressions.Regex.IsMatch(password, "[a-z]"))
            return false;

        // Check for digit
        if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"\d"))
            return false;

        // Check for special character
        if (!System.Text.RegularExpressions.Regex.IsMatch(password, "[!@#$%^&*()_+=-{};:'<>,./?]"))
            return false;

        // Password meets all criteria
        return true;
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
