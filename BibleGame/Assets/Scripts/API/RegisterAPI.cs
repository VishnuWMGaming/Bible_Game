using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RestAPI;

using System;
using Newtonsoft.Json;


namespace BibleGame
{
    namespace API
    {
        public class RegisterAPI : ApiBase
        {
            private static string registerAPI = ServiceURL.baseURL + ServiceURL.signupURL;

            public delegate void RegisterCallBack(bool success, RegisterResponse response = null);

            public static void RegisterUser(RegisterData registerData, RegisterCallBack registerCallBack)
            {
                var jsonData = JsonConvert.SerializeObject(registerData);
                Debug.Log("RequestData:" +  jsonData);
                WebRequest(registerAPI, jsonData, (url, success, data) => HandleResponse(success, data, registerCallBack));
            }

            private static void HandleResponse(bool aSuccess, object data, RegisterCallBack registerCallBack)
            {
                Debug.Log(data.ToString());
                var response = JsonConvert.DeserializeObject<RegisterResponse>(data.ToString());
                if (aSuccess)
                {
                    if (response != null)
                    {
                        if (response.succeeded)
                        {
                            var token = response.ResponseData.token;
                            SetAuthToken(token);
                            registerCallBack?.Invoke(true, response);
                        }
                        else
                        {
                            registerCallBack?.Invoke(false, response);
                        }
                    }
                    else
                    {
                        registerCallBack?.Invoke(false);
                    }
                }
                else
                {
                    registerCallBack?.Invoke(false, response);
                }
            }
        }

        #region REGISTER_DATA
        [Serializable]
        public class RegisterData
        {
            public string name;
            public string email;
            public string password;

            public RegisterData(string fullname, string email, string password)
            {
                this.name = fullname;
                this.email = email;
                this.password = password;
            }
        }
        #endregion

        #region RESPONSE_DATA
        public class RegisterResponse : ResponseBase
        {
            public ResponseBody ResponseData;
        }

        public class ResponseBody
        {
            public string token;
            public string otp;
        }

        #endregion
    }
}
