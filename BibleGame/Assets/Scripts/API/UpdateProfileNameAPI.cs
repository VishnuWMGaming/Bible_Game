using Newtonsoft.Json;
using RestAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DebugUtils;

namespace BibleGame
{

    namespace API
    {
        public class ProfileAPI : ApiBase
        {
            private static string profileNameURL = ServiceURL.baseURL + ServiceURL.profileName;

            public delegate void ProfileCallback(bool success);


            public delegate void ProfilePicCallback(bool success);

            public static void UpdateName(nameDATA name, ProfileCallback callback)
            {
                Debug.Log("Name 3:" + name.name);

                var jsonData = JsonConvert.SerializeObject(name);
                Debug.Log(jsonData);

                WebRequest(profileNameURL, jsonData, (url, success, data) => HandleResponse(success, data, callback));
            }

            private static void HandleResponse(bool aSuccess, object data, ProfileCallback ProfileCallback)
            {
                Debug.Log(data.ToString());
                var response = JsonConvert.DeserializeObject<ProfileNameResponse>(data.ToString());

                if(aSuccess)
                {
                    Debug.Log(data);
                    ProfileCallback?.Invoke(response.succeeded);
                }
            }

            public static void EditPic(ProfilePicCallback callback, WWWForm form)
            {
                string url = $"{ServiceURL.baseURL}{ServiceURL.updateProfilePic}";

                DevDebug.Log($"Update profile pic : {url}",DebugColor.Cyan);

                WebRequest(url, form, (url, success, adata) => 
                {
                    DevDebug.Log($"Update profile pic response: {adata.ToString()}", DebugColor.Orange);

                    if(!success)
                    {
                        callback?.Invoke(false);
                        Debug.LogError("No success at the updating the profile pic ");
                        return;
                    }

                    var data = JsonConvert.DeserializeObject<UpdateProfilePic>(adata.ToString());

                    if(data == null)
                    {
                        callback?.Invoke(false);
                        Debug.LogError("update profile pic can not deserialised..");
                        return;
                    }

                    if(data.ResponseCode != 200)
                    {
                        callback?.Invoke(false);
                        Debug.LogError("update profile pic can not  called ..");
                        return;
                    }

                    callback?.Invoke(true);    
                });
            }
        }

        #region RESQUEST_DATA

        [Serializable]
        public class  nameDATA
        {
           public string name;

            public nameDATA(string name)
            {
                this.name = name;
            }
        }

        #endregion

        #region RESPONSE_DATA

        public class ProfileNameResponse: ResponseBase
        {
            public profileResponseBody ResponseData;
        }

        public class profileResponseBody
        {
            public bool acknowledged;
        }

        public class UpdateProfilePic: ResponseBase
        {

        }

        #endregion
    }
}
