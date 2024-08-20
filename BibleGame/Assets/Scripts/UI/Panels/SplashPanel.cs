using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using BibleGame;

public class SplashPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] Button button;

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnEnable()
    {
        button.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.login));
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }
}
