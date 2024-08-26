using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using BibleGame.Data;
using BibleGame.API;
using BibleGame;

public class VerificationPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] TMP_InputField otpInputField;

    [SerializeField] List<TMP_Text> otps = new List<TMP_Text>();

    [SerializeField] Button verifyBtn;
    [SerializeField] Button backBtn;


    /// <summary>
    /// Action implemented in enable 
    /// </summary>
    private void OnEnable()
    {
        otpInputField.onValueChanged.AddListener(OnValueChanged_Action);
        Debug.Log("OTP >>>" + AppData.otpData.Otp);

        verifyBtn.onClick.AddListener(VerifyAction);
        backBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.signup));
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        otpInputField.onValueChanged.RemoveAllListeners();
        verifyBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();   

        verifyBtn.interactable = false;
    }

    private void VerifyAction()
    {
       var otpData = new OtpData(AppData.otpData.Otp);
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

}
