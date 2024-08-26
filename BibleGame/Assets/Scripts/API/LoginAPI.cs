using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RestAPI;
using Newtonsoft.Json;
using System;

namespace BibleGame
{
    namespace API
    {
        public class LoginAPI : ApiBase
        {
            private static string LogintURL = ServiceURL.baseURL + ServiceURL.login;

            public delegate void loginCallback(bool success, LoginResponseX response = null);

            public static void Login(LoginRequest login, loginCallback callback)
            {
                var jsonData = JsonConvert.SerializeObject(login);

                Debug.Log("login data :" + jsonData);

                WebRequest(LogintURL, jsonData, (url, success, data) => HandleResponse(success, data, callback));
            }

            private static void HandleResponse(bool aSuccess, object data, loginCallback callback)
            {
                Debug.Log(data.ToString());
                var response = JsonConvert.DeserializeObject<LoginResponseX>(data.ToString());


                if (aSuccess)
                {
                    Debug.Log(data.ToString());
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
                        Debug.LogError("Resonse is null");
                        //callback?.Invoke(false, response);
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
        public class LoginRequest
        {
            public string username;
            public string password;

            public LoginRequest (string username, string password)
            {
                this.username = username;
                this.password = password;
            }
        }

        #endregion

        #region RESPONSE_DATA

        public class LoginResponseX: ResponseBase
        {
            public LoginDataX ResponseData;
        }

        public class LoginDataX
        {
            public string token;
            public string name;
        }

        #endregion
    }
}

