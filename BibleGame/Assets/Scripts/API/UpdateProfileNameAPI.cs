using DebugUtils;
using Newtonsoft.Json;
using RestAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace BibleGame
{

    namespace API
    {
        public class ProfileAPI : ApiBase
        {
            private static string profileNameURL = ServiceURL.baseURL + ServiceURL.profileName;

            public delegate void ProfileCallback(bool success);

            public delegate void ProfileDeleteCallback(bool success);

            public delegate void ProfilePicCallback(bool success);

            public static void UpdateName(nameDATA name, ProfileCallback callback)
            {
                var jsonData = JsonConvert.SerializeObject(name);
                Debug.Log($"Edit profile :{jsonData}");

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


            public static async void DeleteProfile(ProfileDeleteCallback callback)
            {
                string url = $"{ServiceURL.baseURL}{ServiceURL.deleteProfile}";

                DevDebug.Log($"Delete profile:{url} ::: {mToken}",DebugColor.Magenta);

                var result = await DeleteProfileAsync(url, mToken);

                DevDebug.Log($"Deleted Response: {result.response.ToString()}", DebugColor.Orange);

                callback?.Invoke(result.success);
            }

            public static async Task<(bool success, string response)> DeleteProfileAsync(
            string serverUrl,
            string token)
            {
                string url = serverUrl;

                using (UnityWebRequest request = UnityWebRequest.Delete(url))
                {
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("authorization", "bearer " + token);

                    var operation = request.SendWebRequest();

                    // 🔄 Await without blocking main thread
                    while (!operation.isDone)
                        await Task.Yield();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        return (true, request.downloadHandler.text);
                    }
                    else
                    {
                        return (false, request.error);
                    }
                }
            }
        }

        #region RESQUEST_DATA

        [Serializable]
        public class  nameDATA
        {
           public string name;
           public string church;

            public nameDATA(string name,string church)
            {
                this.name = name;
                this.church = church;
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
