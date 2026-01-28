using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

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

    [SerializeField] TMP_Text otp;


    float timer;

    /// <summary>
    /// Action implemented in enable 
    /// </summary>
    private void OnEnable()
    {
        otpInputField.onValueChanged.AddListener(OnValueChanged_Action);

        string otpvalue = AppData.otpPage switch
        {
            OTPType.forget => AppData.mforgetotpData.Otp,
            OTPType.sign => AppData.mSignOtpData.Otp
        };

        Debug.Log("OTP >>>" + otpvalue);
        otp.text = otpvalue;

        //Notifications.Instance.SendNotification("OTP", "your otp " + AppData.otpData.Otp);

        verifyBtn.onClick.AddListener(VerifyAction);
        verifyBtn.interactable = false;

        backBtn.onClick.AddListener(() => {

            AudioManager.Instance.PlayButton();
            Actions.ChangePanelActions(AppData.otpPage switch
            {
                OTPType.sign => CanvasType.signup,
                OTPType.forget => CanvasType.forgetpassword,
                _ => throw new ArgumentOutOfRangeException(nameof(AppData.otpPage), AppData.otpPage, null)
            });
        });

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

        otp.text = "";

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

            switch (AppData.otpPage)
            {
                case OTPType.sign:
                    AppData.loginData = new LoginData(AppData.loginData.Email, AppData.loginData.Password, response.ResponseData.name, "");
                    GetProfile(); 
                    Actions.ChangePanelActions(CanvasType.home); break;

                case OTPType.forget:
                    Debug.Log("Forget password page !!!");
                    Actions.ChangePanelActions(CanvasType.updatepassword); break;
            }
        }
        else
        {
            Debug.LogError("Response failed !!!" + response != null ? response.ResponseMessage : "Network issue");
            PopUp.Instance.ShowMessage( response != null ? response.ResponseMessage : "Network issue");
        }
    }
    #endregion
    
    private void GetProfile()
    {
        GetProfileAPI.GetProfile(GetProfileCallback);
    }

    private void GetProfileCallback(bool success, GetProfileResponse response)
    {
        if (success)
        {
            AppData.loginData = new LoginData(response.ResponseData.email, "**********", response.ResponseData.name, response.ResponseData.church);
           // GetChapters();
        }
        else
        {
            PopUp.Instance.EnableLoad(false);
        }
    }
    //private void GetChapters()
    //{
    //    GetChaptersAPI.GetChapters(GetChaptersCallback);
    //}

    private void GetChaptersCallback(bool success, GetChaptersResponse response)
    {
        PopUp.Instance.EnableLoad(false);
        if (success)
        {
            //List<Chapter> chapters = new List<Chapter>();
            //foreach (var chapter in response.ResponseData)
            //{
            //    Chapter obj = new Chapter(chapters.Count, chapter.id, chapter.name, chapter.description);
            //    chapters.Add(obj);
            //}
            Actions.ChangePanelActions(CanvasType.home);
           // GameData.SetChapters(chapters);
        }
    }

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
        resendBtn.onClick.AddListener(()=> { AudioManager.Instance.PlayButton(); ResendOTPAction(); });
        
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

            PopUp.Instance.ShowMessage("OTP resent on your registered email");

           // OTPType oTPType = AppData.otpData.OTPType;
            //AppData.otpData = new OTPData(response.ResponseData.otp, oTPType);

            otp.text = response.ResponseData.otp;

           // Notifications.Instance.SendNotification("OTP", "your otp " + AppData.otpData.Otp);

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
