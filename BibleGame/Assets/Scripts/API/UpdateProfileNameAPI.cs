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
        public class UpdateProfileNameAPI : ApiBase
        {
            private static string profileNameURL = ServiceURL.baseURL + ServiceURL.profileName;

            public delegate void ProfileCallback(bool success);


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

        #endregion
    }
}
