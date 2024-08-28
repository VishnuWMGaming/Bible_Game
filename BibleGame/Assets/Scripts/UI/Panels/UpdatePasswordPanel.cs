using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using BibleGame;
using BibleGame.API;
using System;

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
            error_message = "Please enter the confirm password";
            isValid = false;
        }
        else if (password.Length < 8)
        {
            error_message = "Password length must be 8 character";
            isValid = false;
        }
        else if (password != confirmPassword)
        {
            error_message = "Password does not match";
            isValid = false;
        }

        return isValid;
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
            PopUp.Instance.ShowMessage("Password is updated !!!");

            Actions.ChangePanelActions(CanvasType.login);
        }
        else
        {
            PopUp.Instance.ShowMessage(response.ResponseMessage);
        }
    }
}
