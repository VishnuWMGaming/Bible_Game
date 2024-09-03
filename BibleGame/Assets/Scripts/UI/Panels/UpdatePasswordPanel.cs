using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using BibleGame;
using BibleGame.API;
using System;
using System.Text.RegularExpressions;

public class UpdatePasswordPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] UIPasswordField inputfield_password;
    [SerializeField] UIPasswordField inputField_passwordConfirm;

    [SerializeField] Button updateBtn;
    [SerializeField] Button backBtn;

    string error_message;

    /// <summary>
    /// Action implemented on enable
    /// </summary>
    private void OnEnable()
    {
       updateBtn.onClick.AddListener(UpdateAction);
       backBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.forgetpassword));
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        updateBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();
    }


    private bool ValidatePassword(string password, string confirmPassword)
    {
        bool isValid = true;


        if (password == string.Empty)
        {
            error_message = "Please enter password";
            isValid = false;
        }
        else if(confirmPassword == string.Empty)
        {
            error_message = "Please enter Confirm password";
            isValid = false;
        }
        else if (password.Length < 8)
        {
            error_message = "Password length must be 8 character";
            isValid = false;
        }
        else if(!IsStrongPassword(password))
        {
            error_message = "Password should be between 8 to 16 characters and should include 1 Uppercase, 1 Lowercase, 1 Number and 1 Special Character";
            isValid = false;
        }
        else if (password != confirmPassword)
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

    private void UpdateAction()
    {
       var validatePasword = ValidatePassword(inputfield_password.Text,inputField_passwordConfirm.Text);

        if (!validatePasword)
        {
            PopUp.Instance.ShowMessage(error_message);
            return;
        }

        PopUp.Instance.EnableLoad(true);

        var passwordData = new PasswordData(inputField_passwordConfirm.Text);
        UpdatePasswordAPI.UpdatePassword(passwordData, Callback);
    }

    private void Callback(bool success, UpdatePasswordResponse response)
    {
        PopUp.Instance.EnableLoad(false);

        if (success)
        {
            PopUp.Instance.ShowMessage("Your password has been updated successfully");

            Actions.ChangePanelActions(CanvasType.login);
        }
        else
        {
            PopUp.Instance.ShowMessage(response.ResponseMessage);
        }
    }
}
