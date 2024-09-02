using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using BibleGame.Data;
using BibleGame.API;
using BibleGame;
using System;

public class VerificationPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] TMP_InputField otpInputField;

    [SerializeField] List<TMP_Text> otps = new List<TMP_Text>();

    [SerializeField] Button verifyBtn;
    [SerializeField] Button backBtn;
    [SerializeField] Button resendBtn;

    [SerializeField] TMP_Text timerText;
    [SerializeField] GameObject resendCodeText;


    float timer;

    /// <summary>
    /// Action implemented in enable 
    /// </summary>
    private void OnEnable()
    {
        otpInputField.onValueChanged.AddListener(OnValueChanged_Action);
        Debug.Log("OTP >>>" + AppData.otpData.Otp);

        Notifications.Instance.SendNotification("OTP", "your otp " + AppData.otpData.Otp);

        verifyBtn.onClick.AddListener(VerifyAction);
        verifyBtn.interactable = false;

        backBtn.onClick.AddListener(() => Actions.ChangePanelActions( AppData.otpData.OTPType switch
            { 
               OTPType.sign => CanvasType.signup,
               OTPType.forget => CanvasType.forgetpassword
            }));


        DisplayTimer("00:00");

        resendBtn.gameObject.SetActive(false);

        StopAllCoroutines();
        StartCoroutine(TimerCoroutine(120));
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        otpInputField.onValueChanged.RemoveAllListeners();
        verifyBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();
        resendBtn.onClick.RemoveAllListeners();

        verifyBtn.interactable = false;

        for (int i = 0; i < otps.Count; i++)
        {
            otps[i].text = string.Empty;
        }

        StopAllCoroutines();
    }

    private void VerifyAction()
    {
       var otpData = new OtpData(otpInputField.text);
        OtpAPI.VerifyOtp(otpData, OTPCallback);

        PopUp.Instance.EnableLoad(true);
    }

    #region API
    private void OTPCallback(bool sucess, VerifyOtpResponse response)
    {
        PopUp.Instance.EnableLoad(false);

        if(sucess && response.succeeded)
        {
            Debug.LogWarning("otp verified !!!");

            switch(AppData.otpData.OTPType)
            {
                case OTPType.sign: Actions.ChangePanelActions(CanvasType.home); break;
                case OTPType.forget: Actions.ChangePanelActions(CanvasType.updatepassword); break;
            }
        }
        else
        {
            Debug.LogError("Response failed !!!" + response != null ? response.ResponseMessage : "Network issue");
            PopUp.Instance.ShowMessage("Response failed !!!" + response != null ? response.ResponseMessage : "Network issue");
        }
    }
    #endregion

    void OnValueChanged_Action(string data)
    {
        var otpArray = data.ToCharArray();

        for (int i = 0; i < otps.Count; i++)
        {
            otps[i].text = string.Empty;
        }

        for (int i = 0; i < otpArray.Length; i++)
        {
            otps[i].text = otpArray[i].ToString();
        }

        verifyBtn.interactable = otpArray.Length == 4;
    }

    #region TIMER
    IEnumerator TimerCoroutine(float duration)
    {
        float timer = duration;

        while (timer > 0)
        {
            yield return new WaitForSeconds(1); // Wait for 1 second
            timer--;

           int minutes = Mathf.FloorToInt(timer / 60);
           int seconds = Mathf.FloorToInt(timer % 60);

           string timerText = string.Format("{0:00}:{1:00}", minutes, seconds);
           DisplayTimer(timerText); 
        }

        resendBtn.gameObject.SetActive(true);
        resendBtn.onClick.AddListener(ResendOTPAction);
        
        resendCodeText.gameObject.SetActive(false);

        // ResendOtpAPI.Resend(resendCallback);
    }

    void ResendOTPAction()
    {
        ResendOtpAPI.Resend(resendCallback);

        resendBtn.onClick.RemoveAllListeners();
        resendBtn.gameObject.SetActive(false);

        resendCodeText.gameObject.SetActive(true);
    }


    private void resendCallback(bool success, ResendOtpResponse response)
    {
        if (success)
        {
            Debug.LogWarning("OTP RESENDED");

            PopUp.Instance.ShowMessage("OTP Resended");

            OTPType oTPType = AppData.otpData.OTPType;
            AppData.otpData = new OTPData(response.ResponseData.otp, oTPType);

            Notifications.Instance.SendNotification("OTP", "your otp " + AppData.otpData.Otp);

            StartCoroutine(TimerCoroutine(180));
        }
        else
        {
            Debug.LogError("Otp not send " +  response.ResponseMessage);

            PopUp.Instance.ShowMessage("Otp not send " + response.ResponseMessage);
        }
    }

    void DisplayTimer(string text)
    {
        timerText.text = text;
    }
    #endregion

}
