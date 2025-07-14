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
        public class StreakAPI: ApiBase
        {
            public delegate void CreatStreakCallback(bool success, CreateStreakResponse response = null);

            public static void Create(CreatStreakCallback callback, StreakRequest request)
            {
                var url = $"{ServiceURL.baseURL}{ServiceURL.createStreak}";

                string jsonData = JsonConvert.SerializeObject(request);

                WebRequest(url, jsonData, (url, success, adata) =>
                {
                    if(!success)
                    {
                        Debug.LogError("No success in creat streak");
                        callback?.Invoke(false, null);
                        return;
                    }

                    try
                    {
                        var data = JsonConvert.DeserializeObject<CreateStreakResponse>(adata.ToString());
                        callback?.Invoke(success, data);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error in create strick: {ex.Message}");
                        callback?.Invoke(false, null);
                    }
                });
            }


        }


        #region REQUEST

        [Serializable]
        public class StreakRequest
        {
            public string book_id;
            public string bible_id;
            public string age;
            public string testament;
        }
        #endregion

        #region RESPONSE

        public class CreateStreakResponse:ResponseBase
        {
            public StreakData ResponseData;
        }

        [Serializable]
        public class StreakData
        {
            public string _id;
            public int coins;
        }

        #endregion
    }
}

