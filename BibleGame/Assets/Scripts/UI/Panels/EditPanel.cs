using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using BibleGame.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        church_InputField.onValueChanged.AddListener(SaveChecKAction);

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
        name_InputField.onValueChanged.RemoveAllListeners();
        church_InputField.onValueChanged.RemoveAllListeners();
    }


    void SavePic()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                PopUp.Instance.EnableLoad(true);
                // Load image data from file
                byte[] imageData = System.IO.File.ReadAllBytes(path);
                // Optional: Get file name
                string fileName = System.IO.Path.GetFileName(path);

                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize: 2048, false);

                if (texture == null)
                {
                    PopUp.Instance.EnableLoad(false);

                    Debug.LogError("Couldn't load texture from: " + path);
                    PopUp.Instance.ShowMessage("Unable to save the profile.Please try again");

                    return;
                }

                imageData = texture.EncodeToJPG(70);

                Sprite sprite = Sprite.Create(texture,
                                 new Rect(0, 0, texture.width, texture.height),
                                 new Vector2(0.5f, 0.5f));

                pic.SetRightSize(sprite, true);

                AppData.loginData.UpdateSprite(sprite);

                WWWForm currentForm = new WWWForm();

                string extension = System.IO.Path.GetExtension(fileName).ToLower();
                string mimeType =Utils.GetImageMimeType(extension);

                currentForm.AddBinaryData("image", imageData, fileName, mimeType);

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
        saveBtn.interactable = !String.IsNullOrWhiteSpace(data);
       
        //_name = data;
    }

    private void SaveAction()
    {
        Debug.Log("Name :" +  _name);

       var name =  new nameDATA(name_InputField.text,church_InputField.text);

        Debug.Log($"Name : {name_InputField.text} Church: {church_InputField.text} ");

        PopUp.Instance.EnableLoad(true);
        ProfileAPI.UpdateName(name, (success) =>
        {
            PopUp.Instance.EnableLoad(false);

            if (!success)
            {
                PopUp.Instance.ShowMessage("Profile details not updated.Please Try again");
                Debug.LogError("Someting went wrong");
                return;
            }
           
            Debug.LogWarning("Updated the profile");
            PopUp.Instance.ShowMessage("Profile details updated successfully");

            AppData.loginData = new LoginData(AppData.loginData.Email, 
                                              AppData.loginData.Password,
                                              name_InputField.text,
                                              church_InputField.text);
          
        });
    }
}
