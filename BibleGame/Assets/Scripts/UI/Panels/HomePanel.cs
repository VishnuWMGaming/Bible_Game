using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using BibleGame;
using BibleGame.Data;
public class HomePanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] TMP_Text userName;
    [SerializeField] Button settingBtn;

    /// <summary>
    /// Action implemented on enable
    /// </summary>
    private void OnEnable()
    {
        settingBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.setPanel));

        userName.text = AppData.loginData.Name;
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        settingBtn.onClick.RemoveAllListeners();
    }
}
