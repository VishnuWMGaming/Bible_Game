using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class PopUp : MonoBehaviour
{
   static PopUp instance;
   public static PopUp Instance { get { return instance; } }    

   [SerializeField] TMP_Text messageText;

   [SerializeField] GameObject panel;
   [SerializeField] GameObject loadingPanel;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public void EnableLoad(bool enable)
    {
        loadingPanel.SetActive(enable);
    }

    public void ShowMessage(string message)
    {
       panel.SetActive(true);

       messageText.text = message;   
    }
}
