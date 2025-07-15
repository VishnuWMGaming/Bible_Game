using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class PopUp : MonoBehaviour
{
   static PopUp instance;
   public static PopUp Instance { get { return instance; } }    

   [SerializeField] TMP_Text messageText;

   [SerializeField] GameObject panel;
   [SerializeField] GameObject loadingPanel;
   [SerializeField] Button closeBtn;

   private void OnEnable()
   {
       if (instance == null)
           instance = this;


   }

   private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public void EnableLoad(bool enable)
    {
        loadingPanel.SetActive(enable);
    }

    public void ShowMessage(string message,UnityAction closeAction = null)
    {
        panel.SetActive(true);

        closeBtn?.onClick.RemoveAllListeners();
        closeBtn?.onClick.AddListener(() => panel.SetActive(false));

        if(closeAction != null)
        closeBtn?.onClick.AddListener(closeAction);

        messageText.text = message;   
    }
}
