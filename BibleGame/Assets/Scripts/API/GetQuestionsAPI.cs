using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using RestAPI;
using UnityEngine;

namespace BibleGame
{
    namespace API
    {
        public class GetQuestionsAPI : ApiBase
        {
            public delegate void GetQuestionsObjectiveCallback(bool success, GetQuestionsResponseObjective response = null);
            public delegate void GetQuestionsAnagramCallback(bool success, GetQuestionsResponseAnagram response = null);

            public static void GetQuestionsObjective(GetQuestionsRequestData requestData, GetQuestionsObjectiveCallback callback)
            {
                var url = ServiceURL.baseURL + ServiceURL.getQuestions;

                var jsonData = JsonConvert.SerializeObject(requestData);
                WebRequest(url, jsonData, (url, success, adata) =>
                {
                    if(!success)
                    {
                        Debug.LogError($"No success in getting objective questions {adata.ToString()}");
                        callback?.Invoke(false, null);
                        return;
                    }

                    try
                    {
                        var data = JsonConvert.DeserializeObject<GetQuestionsResponseObjective>(adata.ToString());

                        callback?.Invoke(success, data);
                    }
                    catch(Exception ex)
                    {
                        Debug.LogError($"Error in getting objective questions {ex.Message}");
                        callback?.Invoke(false, null);
                    }
                });
            }

            public static void GetQuestionsAnagram(GetQuestionsRequestData requestData, GetQuestionsAnagramCallback callback)
            {
                var url = ServiceURL.baseURL + ServiceURL.getQuestions;

                var jsonData = JsonConvert.SerializeObject(requestData);
                WebRequest(url, jsonData, (url, success, adata) =>
                {
                    if (!success)
                    {
                        Debug.LogError($"No success in getting anagram questions {adata.ToString()}");
                        callback?.Invoke(false, null);
                        return;
                    }

                    try
                    {
                        var data = JsonConvert.DeserializeObject<GetQuestionsResponseAnagram>(adata.ToString());

                        callback?.Invoke(success, data);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error in getting anagram questions {ex.Message}");
                        callback?.Invoke(false, null);
                    }
                });
            }



        }

        #region REQUEST
        [Serializable]
        public class GetQuestionsRequestData
        {
            public string game_id;
            public string ageGroup;
            public string game_type;
            public string bible_id;
            public string book_id;
            public string chapter_id;
        }
        #endregion

        #region RESPONSE
        public class GetQuestionsResponseObjective : ResponseBase
        {
            public GetQuestionsResponseObjectiveRData ResponseData;
        }

        [Serializable]
        public class GetQuestionsResponseObjectiveRData
        {
            public List<GetQuestionsObjectiveData> questions;
            public LevelData levelData;
        }

        [Serializable]
        public class GetQuestionsObjectiveData
        {
            public string _id;
            public string title;
            public string bible_id;
            public string book_id;
            public string chapter_id;
            public string ageGroup;
            public string opt1;
            public string opt2;
            public string opt3;
            public string opt4;
            public string hint;
        }

        public class GetQuestionsResponseAnagram : ResponseBase
        {
            public GetQuestionsResponseAnagramRData ResponseData;
        }

        [Serializable]
        public class GetQuestionsResponseAnagramRData
        {
            public List<GetQuestionsAnagramData> questions;
            public LevelData levelData;
        }

        [Serializable]
        public class GetQuestionsAnagramData
        {
            public string _id;
            public string bible_id;
            public string book_id;
            public string chapter_id;
            public string ageGroup;
            public string title;
            public string hint;
        }

        [Serializable]
        public class LevelData
        {
            public string _id;
            public string game_id;
            public string chapter_id;
            public int rating;
            public int coin_earn;
        }

        #endregion
    }
}