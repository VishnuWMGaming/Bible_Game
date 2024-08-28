using Newtonsoft.Json;
using RestAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BibleGame.API.RegisterAPI;


namespace BibleGame
{
    namespace API
    {
        public class ResendOtpAPI : ApiBase
        {
            private static string resendAPI = ServiceURL.baseURL + ServiceURL.resendOtp;

            public delegate void ResendCallBack(bool success, ResendOtpResponse response = null);

            public static void Resend(ResendCallBack callBack)
            {
                WebRequest(resendAPI, "", (url, success, data) => HandleResponse(success, data, callBack));
            }

            private static void HandleResponse(bool aSuccess, object data, ResendCallBack resendCallback)
            {
                Debug.Log(data.ToString());
                var response = JsonConvert.DeserializeObject<ResendOtpResponse>(data.ToString());

                if (aSuccess)
                {
                    if (response != null)
                        resendCallback?.Invoke(response.succeeded, response);
                    else
                        Debug.LogError("The response is null");
                }
                else
                    Debug.LogError("Network error ");
            }

            #region REQUEST_DATA

            #endregion

            #region RESPONSE_DATA

            public class ResendOtpResponse : ResponseBase
            {
                public ResendResponseDataX ResponseData;
            }

            public class ResendResponseDataX
            {
                public string otp;
            }

            #endregion
        }
    }
}


