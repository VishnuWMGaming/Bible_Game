using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

using BibleGame.API;

public class EditPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] TMP_InputField name_InputField;
    [SerializeField] TMP_InputField email_InputField;
    [SerializeField] Button saveBtn;

    string _name;

    private void OnEnable()
    {
        email_InputField.interactable = false;
        name_InputField.interactable = true;

        name_InputField.onValueChanged.AddListener(SaveChecKAction);

        saveBtn.interactable = false;
        saveBtn.onClick.AddListener(SaveAction);
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

        UpdateProfileNameAPI.UpdateName(name, APICallback);
    }

    void APICallback(bool success)
    {
        if (success)
        {
            Debug.LogWarning("Updated the name");
            PopUp.Instance.ShowMessage("Updated the name");
        }
        else
            Debug.LogError("Someting went wrong");
    }
}
