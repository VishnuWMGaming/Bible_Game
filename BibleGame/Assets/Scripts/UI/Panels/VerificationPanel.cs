using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class VerificationPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] TMP_InputField otpInputField;

    [SerializeField] List<TMP_Text> otps = new List<TMP_Text>();


    /// <summary>
    /// Action implemented in enable 
    /// </summary>
    private void OnEnable()
    {
        otpInputField.onValueChanged.AddListener(OnValueChanged_Action);            
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        otpInputField.onValueChanged.RemoveAllListeners();
    }

    void OnValueChanged_Action(string data)
    {
        var otpArray = data.ToCharArray();

        for (int i = 0; i < otps.Count; i++)
        {
            otps[i].text = string.Empty;
        }

        for (int i = 0; i < otps.Count; i++)
        {
            otps[i].text = otpArray[i].ToString();
        }
    }

}
