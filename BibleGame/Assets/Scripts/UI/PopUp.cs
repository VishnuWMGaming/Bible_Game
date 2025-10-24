using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using BibleGame.Data;

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

        if (AppData.orientation == null)
            AppData.orientation = MScreenOriatation.portrait;

        loadingPanel_Landscape.SetActive(false);
        loadingPanel_Portrait.SetActive(false);

        Debug.Log($"ORIENTTION : {AppData.orientation}");

        switch(AppData.orientation)
        {
           case MScreenOriatation.landscape:
                loadingPanel_Landscape.SetActive(enable);
                break;

            case MScreenOriatation.portrait:
                loadingPanel_Portrait.SetActive(enable);
                break;
        }
    }

    public void ShowMessage(string message,UnityAction closeAction = null)
    {
        if (AppData.orientation == null)
            AppData.orientation = MScreenOriatation.portrait;

        panel_Portrait.gameObject.SetActive(false);
        panel_Landscape.gameObject.SetActive(false);
        
        PanelPop panelPop = AppData.orientation == MScreenOriatation.portrait ? panel_Portrait : panel_Landscape;

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
