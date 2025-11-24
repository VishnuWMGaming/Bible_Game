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
            public delegate void SubmitAnswerCallback(bool success, SubmitAnswerResponse response = null);


            public static void GetQuestionsObjective(GetQuestionsObjectiveCallback callback, GetQuestionsRequestData requestData)
            {
                var url = ServiceURL.baseURL + ServiceURL.getQuestions;

                string jsonData = JsonConvert.SerializeObject(requestData);

                Debug.Log($"<color=magenta> Getting questions for objective : {url} =>  {jsonData} </color>");

                WebRequest(url, jsonData, (url, success, adata) =>
                {
                    if(!success)
                    {
                        Debug.LogError($"No success in getting objective questions {adata.ToString()}");
                        callback?.Invoke(false, null);
                        return;
                    }

                    Debug.Log($"<color=#FFA500> Question data : {adata.ToString()}</color>");

                    var data = JsonConvert.DeserializeObject<GetQuestionsResponseObjective>(adata.ToString());

                    if(data == null)
                    {
                        Debug.LogError("Unable to serialise the questions");

                        callback?.Invoke(false, null);
                        return;
                    }

                    callback?.Invoke(success, data);
                });
            }

            public static void GetQuestionsAnagram(GetQuestionsAnagramCallback callback, GetQuestionsRequestData requestData)
            {
                var url = ServiceURL.baseURL + ServiceURL.getQuestions;

                var jsonData = JsonConvert.SerializeObject(requestData);

                Debug.Log($"<color=magenta> Getting questions for anagram : {url} =>  {jsonData} </color>");

                WebRequest(url, jsonData, (url, success, adata) =>
                {
                    if (!success)
                    {
                        Debug.LogError($"No success in getting anagram questions {adata.ToString()}");
                        callback?.Invoke(false, null);
                        return;
                    }

                    Debug.Log($"<color=#FFA500> Question data : {adata.ToString()}</color>");

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

            public static void SubmitAnswer(SubmitAnswerCallback callback, SubmitRequestData requestData)
            {
                var url = ServiceURL.baseURL + ServiceURL.submitAnswer;

                var jsonData = JsonConvert.SerializeObject(requestData);

                Debug.Log($"<color=magenta> Submit : {url} =>  {jsonData} </color>");

                WebRequest(url, jsonData, (url, success, adata) =>
                {
                    Debug.Log($"<color=#FFA500> Submit data : {adata.ToString()}</color>");

                    if (!success)
                    {
                        Debug.LogError($"No success in submitting answer {adata.ToString()}");
                        callback?.Invoke(false, null);
                        return;
                    }

                    try
                    {
                        var data = JsonConvert.DeserializeObject<SubmitAnswerResponse>(adata.ToString());
                        callback?.Invoke(success, data);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error in submitting answer {ex.Message}");
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
            public string language;
            public string game_type;
            public string bible_id;
            public string book_id;
            public string chapter_id;
        }

        [Serializable]
        public class SubmitRequestData
        {
            public string level_id;
            public int coins;
            public int ratings;
            public List<string> question_data;
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
            public List<GetQuestionsObjectiveData> resArr;
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
            public int ageGroup;
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
            public List<GetQuestionsAnagramData> resArr;
            public LevelData levelData;
        }

        [Serializable]
        public class GetQuestionsAnagramData
        {
            public string _id;
            public string bible_id;
            public string book_id;
            public string chapter_id;
            public int ageGroup;
            public string title;
            public string hint;
            public int status;
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

        public class SubmitAnswerResponse: ResponseBase
        {
            public SubmitRData streakLevel;
        }

        [Serializable]
        public class SubmitRData
        {
            public streakSubmitRData streakLevel;
        }

        [Serializable]
        public class streakSubmitRData
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