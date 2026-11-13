using DebugUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace BibleGame
{
    namespace API
    {
        public class LoginAPI : ApiBase
        {
            private static string LogintURL = ServiceURL.baseURL + ServiceURL.login;

            private static string guestLoginURL = ServiceURL.baseURL2 + ServiceURL.guestLogin;

            public delegate void guestloginCallback(bool success, LoginResponseX response = null);

            public delegate void loginCallback(bool success, LoginResponseX response = null);

            private  static string GetGuestUUID()
            {
                const string key = "GuestUUID";

                string uuid = PlayerPrefs.GetString(key, "");

                if (string.IsNullOrEmpty(uuid))
                {
                    byte[] bytes = new byte[8];

                    using (var rng = RandomNumberGenerator.Create())
                    {
                        rng.GetBytes(bytes);
                    }

                    ulong randomValue = BitConverter.ToUInt64(bytes, 0);

                    // Generates a 12-digit number (100000000000–999999999999).
                    ulong number = 100000000000UL +
                                   randomValue % 900000000000UL;

                    uuid = number.ToString();

                    PlayerPrefs.SetString(key, uuid);
                    PlayerPrefs.Save();
                }

                return uuid;
            }


            public static void GuestLogin(guestloginCallback callback)
            {
                JObject requestData = new JObject
                {
                    ["uuid"] = GetGuestUUID(),
                    ["name"] = "Guest",
                    ["device_id"] = SystemInfo.deviceUniqueIdentifier
                };

                DevDebug.Log($"Guest login {guestLoginURL} :: {requestData.ToString()}", DebugColor.Orange);

                WebRequest(guestLoginURL, requestData.ToString(), (url, success, data) => 
                {
                    var response = JsonConvert.DeserializeObject<LoginResponseX>(data.ToString());

                    DevDebug.Log($"Guest login response : {data.ToString()} :: {response.ResponseData.token}", DebugColor.Orange);

                    if (!success)
                    {
                        callback?.Invoke(false, null);
                        return;
                    }


                    callback?.Invoke(true, response);

                });
            }

            public static void Login(LoginRequest login, loginCallback callback)
            {
                var jsonData = JsonConvert.SerializeObject(login);

                Debug.Log("login data :" + jsonData);

                WebRequest(LogintURL, jsonData, (url, success, data) => HandleResponse(success, data, callback));
            }

            private static void HandleGuestResponse(bool aSuccess, object data, loginCallback callback)
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
                            PlayerPrefs.SetString("AuthorizationToken", token);
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
                            PlayerPrefs.SetString("AuthorizationToken", token);
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

#nullable enable
        public class LoginDataX
        {
            public bool verified;
            public string? token;
            public string? name;
            public string? otp;
        }

        #endregion
    }
}

