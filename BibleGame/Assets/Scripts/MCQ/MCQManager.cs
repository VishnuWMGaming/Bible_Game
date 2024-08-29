using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using UnityEditor.PackageManager;

public class MCQManager : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] OptionPanel optionPanel;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Button _homeBtn;
    [SerializeField] Button _settingBtn;

    /// <summary>
    /// Action implemented one enable
    /// </summary>
    private void OnEnable()
    {
        
    }

    /// <summary>
    /// Action imeplemented om disable
    /// </summary>
    private void OnDisable()
    {
        _homeBtn.onClick.RemoveAllListeners();  
        _settingBtn.onClick.RemoveAllListeners();
    }
}
