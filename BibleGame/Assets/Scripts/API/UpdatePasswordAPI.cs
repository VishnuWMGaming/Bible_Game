using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BibleGame;
using System;
using Newtonsoft.Json;

using RestAPI;
using static BibleGame.API.OtpAPI;

namespace BibleGame
{
    namespace API
    {
        public class UpdatePasswordAPI : ApiBase
        {
            private static string UpdateURL = ServiceURL.baseURL + ServiceURL.updatePassword;

            public delegate void UpdateCallback(bool success, UpdatePasswordResponse response = null);

            public static void UpdatePassword(PasswordData data,UpdateCallback callback)
            {
                var jsonData = JsonConvert.SerializeObject(data);

                Debug.Log("Update password" + jsonData);
                WebRequest(UpdateURL, jsonData, (url, success, data) => HandleResponse(success, data, callback));
            }

            private static void HandleResponse(bool aSuccess, object data, UpdateCallback callback)
            {
                Debug.Log(data.ToString());
                var response = JsonConvert.DeserializeObject<UpdatePasswordResponse>(data.ToString());

                if(aSuccess)
                {
                    if(response != null)
                    {
                        if(response.ResponseData.acknowledged)
                        {
                            callback?.Invoke(true);
                        }
                    }
                    else
                    {
                        Debug.LogError("Response is null");
                        callback?.Invoke(false,null);
                    }
                }
                else
                {
                    callback?.Invoke(false,response);
                }
            }

        }
    }

    #region REQUEST_DATA

    [Serializable]
    public class PasswordData
    {
        public string password;

        public PasswordData (string password)
        {
            this.password = password;
        }   
    }

    #endregion

    #region RESPONSE_DATA

    public class UpdatePasswordResponse :ResponseBase
    {
        public ResponseDataX ResponseData;
    }

    public class ResponseDataX
    {
        public bool acknowledged;
    }

    #endregion
}