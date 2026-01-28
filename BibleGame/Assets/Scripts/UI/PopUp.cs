using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using BibleGame.Data;
using DebugUtils;

public class PopUp : MonoBehaviour
{
   static PopUp instance;
   public static PopUp Instance { get { return instance; } }    

   [SerializeField] TMP_Text messageText;

   [SerializeField] PanelPop panel_Portrait;
   [SerializeField] PanelPop panel_Landscape;

   [SerializeField] GameObject loadingPanel_Portrait;
   [SerializeField] GameObject loadingPanel_Landscape;

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
        //loadingPanel.SetActive(enable);

        //loadingPanel_Portrait.SetActive(false);

       // DevDebug.Log($"LoadingP .. {enable}", DebugColor.Brown);
        loadingPanel_Portrait.SetActive(enable);
    }

    public void ShowMessage(string message,UnityAction closeAction = null,MScreenOriatation mScreenOriatation = MScreenOriatation.portrait)
    {
        if (AppData.orientation == null)
            AppData.orientation = MScreenOriatation.portrait;

        panel_Portrait.gameObject.SetActive(false);
        panel_Landscape.gameObject.SetActive(false);

        PanelPop panelPop = mScreenOriatation == MScreenOriatation.portrait? panel_Portrait : panel_Landscape;

        panelPop.gameObject.SetActive(true);

        panelPop.CloseBtn?.onClick.RemoveAllListeners();
        panelPop.CloseBtn?.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); panelPop.gameObject.SetActive(false); });

        if (closeAction != null)
            panelPop.CloseBtn?.onClick.AddListener(closeAction);

        panelPop.messageTxt.text = message;

        //closeBtn?.onClick.RemoveAllListeners();
        //closeBtn?.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); panel.SetActive(false); });

        //if(closeAction != null)
        //closeBtn?.onClick.AddListener(closeAction);

        //messageText.text = message;   
    }
}
