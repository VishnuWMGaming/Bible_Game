using Newtonsoft.Json;
using RestAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BibleGame
{
    namespace API
    {
        public class OtpAPI : ApiBase
        {
            private static string VerifyUrl = ServiceURL.baseURL + ServiceURL.verify;

            public delegate void VerifyOtpCallback(bool success, VerifyOtpResponse response = null);

            public static void VerifyOtp(OtpData verifyOtpData, VerifyOtpCallback verifyOtpCallback)
            {
                var jsonData = JsonConvert.SerializeObject(verifyOtpData);
                Debug.Log("OTP DATA" + jsonData);
                WebRequest(VerifyUrl, jsonData, (url, success, data) => HandleResponse(success, data, verifyOtpCallback));
            }

            private static void HandleResponse(bool aSuccess, object data, VerifyOtpCallback verifyOtpCallback)
            {
                Debug.Log(data.ToString());
                var response = JsonConvert.DeserializeObject<VerifyOtpResponse>(data.ToString());
                if (aSuccess)
                {
                    Debug.Log(data);
                    if (response != null)
                    {
                        if (response.succeeded)
                        {
                            var token = response.ResponseData.token;
                            SetAuthToken(token);
                            verifyOtpCallback?.Invoke(true, response);
                        }
                        else
                        {
                            verifyOtpCallback?.Invoke(false, response);
                        }
                    }
                    else
                    {
                        verifyOtpCallback?.Invoke(false, response);
                    }
                }
                else
                {
                    verifyOtpCallback?.Invoke(false, response);
                }
            }
        }


        #region REQUEST DATA
        [Serializable]
        public class OtpData
        {
            public string otp;

            public OtpData(string otpData)
            {
                otp = otpData;
            }
        }

        #endregion

        #region RESPONSE DATA
        public class VerifyOtpResponse : ResponseBase
        {
            public OtpResponseBody ResponseData;
        }

        public class OtpResponseBody
        {
            public string token;
        }
        #endregion
    }
}
