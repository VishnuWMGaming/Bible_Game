using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestAPI;
using UnityEngine;

namespace BibleGame
{
    namespace API
    {
        public class GetProfileAPI : ApiBase
        {
            private static string GetProfileURL = ServiceURL.baseURL + ServiceURL.getProfile;

            public delegate void GetProfileCallback(bool success, GetProfileResponse response = null);


            public static GetProfileResponse_ profilData;

            public static void GetProfile(GetProfileCallback callback)
            {
                WebRequestGet(GetProfileURL, (url, success, data) => HandleGetProfileCallback(success, data, callback));
            }

            private static void HandleGetProfileCallback(bool aSuccess, object aData, GetProfileCallback callback)
            {
                Debug.Log("Get Profile: " + aData.ToString());
                var response = JsonConvert.DeserializeObject<GetProfileResponse>(aData.ToString());

                if (aSuccess)
                {
                    profilData = response.ResponseData;
                    callback?.Invoke(response.succeeded, response);
                }
                else
                {
                    
                }

                PopUp.Instance.EnableLoad(false);
            }
        }

        public class GetProfileResponse : ResponseBase
        {
            public GetProfileResponse_ ResponseData;
        }
        
        public class GetProfileResponse_
        {
            public string user_id;
            public string name;
            public string email;
            public string profile_pic;
            public string church;
        }
    }
}