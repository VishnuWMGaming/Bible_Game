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
using BibleGame.Utility;

public class EditPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] TMP_InputField name_InputField;
    [SerializeField] TMP_InputField email_InputField;
    [SerializeField] TMP_InputField church_InputField;

    [Space]
    [SerializeField] Button saveBtn;
    [SerializeField] Button backbtn;
    [SerializeField] Button saveProfile;

    [Space]
    [SerializeField] PostImage pic;

    string _name;

    public UnityEvent CloseAction;

    private void OnEnable()
    {
        email_InputField.interactable = false;
        name_InputField.interactable = true;

        email_InputField.text = AppData.loginData.Email;
        name_InputField.text = AppData.loginData.Name;
        church_InputField.text = AppData.loginData.Church;

        name_InputField.onValueChanged.AddListener(SaveChecKAction);

        saveBtn.interactable = false;
        saveBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); SaveAction(); });

        backbtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.setPanel); });
        backbtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); CloseAction?.Invoke(); });
        backbtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); gameObject.SetActive(false); });

        saveProfile?.onClick.AddListener(SavePic);

        if(AppData.loginData.Pic != null) 
         pic.SetRightSize(AppData.loginData.Pic);
    }

    private void OnDisable()
    {
        saveBtn.onClick.RemoveAllListeners();
        saveProfile?.onClick.RemoveAllListeners();
    }


    void SavePic()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {

            if (path != null)
            {
                // Load image data from file
                byte[] imageData = System.IO.File.ReadAllBytes(path);
                // Optional: Get file name
                string fileName = System.IO.Path.GetFileName(path);

                Sprite sprite =  Utils.LoadSpriteFromBytes(imageData);
                pic.SetRightSize(sprite, true);

                AppData.loginData.UpdateSprite(sprite);

                WWWForm currentForm = new WWWForm();

                string extension = System.IO.Path.GetExtension(fileName).ToLower();
                string mimeType =Utils.GetImageMimeType(extension);

                currentForm.AddBinaryData("image", imageData, fileName, mimeType);

                PopUp.Instance.EnableLoad(true);
                ProfileAPI.EditPic((success) =>
                {
                    PopUp.Instance.EnableLoad(false);
                    if (!success)
                    {
                        PopUp.Instance.ShowMessage("Error in Updating the profile");
                        return;
                    };

                    PopUp.Instance.ShowMessage("Profile pic updated successfully");

                }, currentForm);
               
            }

        }, "Select an image", "image/*");
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
        ProfileAPI.UpdateName(name, APICallback);
    }

    void APICallback(bool success)
    {
        PopUp.Instance.EnableLoad(false);

        if (success)
        {
            Debug.LogWarning("Updated the name");
            PopUp.Instance.ShowMessage("Profile details updated successfully");

            AppData.loginData = new LoginData( AppData.loginData.Email,AppData.loginData.Password,_name,AppData.loginData.Church);
        }
        else
            Debug.LogError("Someting went wrong");
    }
}
