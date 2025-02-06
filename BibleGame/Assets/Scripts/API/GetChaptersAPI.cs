using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestAPI;
using UnityEngine;

namespace BibleGame
{
    namespace API
    {
        public class GetChaptersAPI : ApiBase
        {
            private static string GetChaptersURL = ServiceURL.baseURL + ServiceURL.getChapters;

            public delegate void GetChapterCallback(bool success, GetChaptersResponse response = null);

            public static void GetChapters(GetChapterCallback callback)
            {
                WebRequestGet(GetChaptersURL, (url, success, data) => HandleGetCbhapters(success, data, callback));
            }

            private static void HandleGetCbhapters(bool aSuccess, object aData, GetChapterCallback callback)
            {
                Debug.Log("Get Chapters: " + aData.ToString());
                if (aSuccess)
                {
                    var response = JsonConvert.DeserializeObject<GetChaptersResponse>(aData.ToString());
                    callback?.Invoke(aSuccess, response);
                }
                else
                {
                    //Todo: show error
                }
            }
        }

        public class GetChaptersResponse : ResponseBase
        {
            public List<GetChaptersResponseData> ResponseData;
        }

        public class GetChaptersResponseData
        {
            public string id;
            public string name;
            public string description;
        }
    }
}