using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

using BibleGame.API;
using BibleGame;
using BibleGame.Data;
using UnityEngine.Events;

public class EditPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] TMP_InputField name_InputField;
    [SerializeField] TMP_InputField email_InputField;
    [SerializeField] Button saveBtn;
    [SerializeField] Button backbtn;

    string _name;

    public UnityEvent CloseAction;

    private void OnEnable()
    {
        email_InputField.interactable = false;
        name_InputField.interactable = true;

        email_InputField.text = AppData.loginData.Email;
        name_InputField.text = "";

        name_InputField.onValueChanged.AddListener(SaveChecKAction);

        saveBtn.interactable = false;
        saveBtn.onClick.AddListener(SaveAction);

        backbtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.setPanel));
        backbtn.onClick.AddListener(() => CloseAction?.Invoke());
        backbtn.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void OnDisable()
    {
        saveBtn.onClick.RemoveAllListeners();
    }


    private void SaveChecKAction(string data)
    {
        saveBtn.interactable = !String.IsNullOrEmpty(data);
       
        _name = data;
    }

    private void SaveAction()
    {
        Debug.Log("Name :" +  _name);

       var name =  new nameDATA(_name);

        Debug.Log("Name :" + name.name);

        PopUp.Instance.EnableLoad(true);
        UpdateProfileNameAPI.UpdateName(name, APICallback);
    }

    void APICallback(bool success)
    {
        PopUp.Instance.EnableLoad(false);

        if (success)
        {
            Debug.LogWarning("Updated the name");
            PopUp.Instance.ShowMessage("Updated the name");

            AppData.loginData = new LoginData( AppData.loginData.Email,AppData.loginData.Password,_name);
        }
        else
            Debug.LogError("Someting went wrong");
    }
}
