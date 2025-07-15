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
            public delegate void GetStreakCallback(bool success, GetStreakResponse response = null);
            public delegate void GetStreakDetailCallback(bool success, GetStreakDetail response = null);

            public static List<StreakData> streakDatas;

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


            public static void Get(GetStreakCallback callback)
            {
                var url = $"{ServiceURL.baseURL}{ServiceURL.getStreak}";

                WebRequestGet(url, (url, success, adata) =>
                {

                    if (!success)
                    {
                        Debug.LogError("No Success in get streaks");
                        callback?.Invoke(false, null);

                        return;
                    }

                    Debug.Log($"<color=#FFA500> Streak data : {adata.ToString()}</color>");

                    try
                    {
                        var data = JsonConvert.DeserializeObject<GetStreakResponse>(adata.ToString());

                        streakDatas = data.ResponseData;

                        callback?.Invoke(success,data);
                    }
                    catch(Exception ex)
                    {
                        Debug.LogError($"Error in get strick: {ex.Message}");
                        callback?.Invoke(false, null);
                    }

                });
            }

            public static void GetDetail(GetStreakDetailCallback callback,string id)
            {
                var url = $"{ServiceURL.baseURL}{ServiceURL.getStreakDetail}";

                string jsonData = JsonConvert.SerializeObject(new { game_id = id });

                Debug.Log($"Streak detail :{url} : {jsonData}");

                WebRequest(url, jsonData, (url, success, adata) =>
                {
                    if (!success)
                    {
                        Debug.LogError("No success in get streak details");
                        callback?.Invoke(false, null);
                        return;
                    }

                    try
                    {
                        var data = JsonConvert.DeserializeObject<GetStreakDetail>(adata.ToString());
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

        public class GetStreakResponse: ResponseBase
        {
            public List<StreakData> ResponseData;
        }

        public class GetStreakDetail:ResponseBase
        {
            public StreakDetailData ResponseData;
        }

        [Serializable]
        public class StreakDetailData
        {
            public StreakData streak;
            public List<LevelData> levels;
        }

        [Serializable]
        public class StreakData
        {
            public string _id;
            public string bible_id;
            public string book_id;
            public string age;
            public string testament;
            public int coins;
        }

        #endregion
    }
}

