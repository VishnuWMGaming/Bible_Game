using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BibleGame.UI;
using BibleGame;
using Unity.VisualScripting;

public class UIController : MonoBehaviour
{
    [Header("CanvaseType")]
    [SerializeField] CanvasType _canvasType;

    GameObject loginCanvas;
    GameObject splashCanvas;
    GameObject signCanvas;
    GameObject forgetPasswordCanvas;
    GameObject otpCanvas;
    GameObject homeCanvas;
    GameObject updatePasswordCanavas;
    GameObject settingsCanvas;
    GameObject currentPanel;
    GameObject chapterPanel;

    /// <summary>
    /// Action called on validation
    /// </summary>
    private void OnValidate()
    {
    }

    /// <summary>
    /// Action implemented on awake
    /// </summary>
    private void Awake()
    {
        // BibleUI.Init(this);


        loginCanvas = FindObjectOfType<LoginPanel>(true).gameObject;
        splashCanvas = FindObjectOfType<SplashPanel>(true).gameObject;
        signCanvas = FindObjectOfType<SignPanel>(true).gameObject;
        forgetPasswordCanvas = FindObjectOfType<ForgetPasswordPanel>(true).gameObject;
        otpCanvas = FindObjectOfType<VerificationPanel>(true).gameObject;
        updatePasswordCanavas = FindObjectOfType<UpdatePasswordPanel>(true).gameObject;
        homeCanvas = FindObjectOfType<HomePanel>(true).gameObject;
        settingsCanvas = FindObjectOfType<SettingsPanel>(true).gameObject;
        chapterPanel = FindObjectOfType<ChapterPanel>(true).gameObject;    

        _canvasType = CanvasType.splash;
        currentPanel = splashCanvas;
        currentPanel.SetActive(true);

        Actions.ChangePanelActions += ChangePanel;
    }

    /// <summary>
    /// Action implemented on changing the panel
    /// </summary>
    /// <param name="canvasType"></param>
     private void ChangePanel(CanvasType canvasType)
     {
        if (currentPanel != null)
            currentPanel.SetActive(false);
        else
        {
            Debug.LogError("Current panel is null");
        }

        _canvasType = canvasType;
 
        currentPanel = canvasType switch { 
                                           CanvasType.login => loginCanvas,
                                           CanvasType.splash => splashCanvas,
                                           CanvasType.signup => signCanvas,
                                           CanvasType.forgetpassword => forgetPasswordCanvas,
                                           CanvasType.otp => otpCanvas,
                                           CanvasType.updatepassword => updatePasswordCanavas,
                                           CanvasType.home => homeCanvas,
                                           CanvasType.setPanel => settingsCanvas,
                                           CanvasType.chapter => chapterPanel,
                                           _=> null
                                         };

        currentPanel.SetActive(true);
     }
}


public enum CanvasType
{
    login,
    signup,
    forgetpassword,
    otp,
    splash,
    updatepassword,
    home,
    setPanel,
    chapter
}
