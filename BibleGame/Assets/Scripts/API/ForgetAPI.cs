using Newtonsoft.Json;
using RestAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static BibleGame.API.OtpAPI;


namespace BibleGame
{
    namespace API
    {
        public class ForgetAPI : ApiBase
        {
            private static string ForgetURL = ServiceURL.baseURL + ServiceURL.forgetPassword;

            public delegate void ForgetCallback(bool success, ForgetResponse response = null);

            public static void Retrive(EmailData emailData, ForgetCallback callback)
            {
                var jsonData = JsonConvert.SerializeObject(emailData);

                Debug.Log("Email data :" + jsonData);
                WebRequest(ForgetURL, jsonData, (url, success, data) => HandleResponse(success, data, callback));
            }

            private static void HandleResponse(bool aSuccess, object data, ForgetCallback callback)
            {
                Debug.Log(data.ToString());
                var response = JsonConvert.DeserializeObject<ForgetResponse>(data.ToString());

                if (aSuccess)
                {
                    Debug.Log(data);
                    if (response != null)
                    {
                        if (response.succeeded)
                        {
                            var token = response.ResponseData.token;
                            SetAuthToken(token);
                            callback?.Invoke(true, response);
                        }
                        else
                        {
                            callback?.Invoke(false, response);
                        }
                    }
                    else
                    {
                        //Debug.LogError("Resonse is null");
                        callback?.Invoke(false, response);
                    }
                }
                else
                {
                    callback?.Invoke(false, response);
                }
            }

        }

        #region REQUEST_DATA

        [Serializable]
        public class EmailData
        {
            public string email;

            public EmailData(string email)
            {
                this.email = email; 
            }
        }
        #endregion

        #region RESPONSE_DATA
        public class ForgetResponse: ResponseBase
        {
            public ForgetData ResponseData;
        }

        public class ForgetData
        {
            public string token;
            public string otp;
        }


        #endregion
    }
}