using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestAPI;
using UnityEngine;

namespace BibleGame
{
    namespace API
    {
        public class GetQuestionsAPI : ApiBase
        {
            private static string GetQuestionsURL = ServiceURL.baseURL + ServiceURL.getQuestions;

            public delegate void GetQuestionsCallback(bool success, GetQuestionsResponse response = null);

            public static void GetQuestions(GetQuestionsRequestData requestData, GetQuestionsCallback callback)
            {
                var jsonData = JsonConvert.SerializeObject(requestData);
                WebRequest(GetQuestionsURL, jsonData, (url, success, data) => HandleGetQuestions(success, data, callback));
            }

            private static void HandleGetQuestions(bool aSuccess, object aData, GetQuestionsCallback callback)
            {
                Debug.Log("Get Questions Response Data: " + aData);
                if (aSuccess)
                {
                    var response = JsonConvert.DeserializeObject<GetQuestionsResponse>(aData.ToString());
                    callback?.Invoke(aSuccess, response);
                }
            }
        }
        

        [Serializable]
        public class GetQuestionsRequestData
        {
            public string chapter_id;

            public GetQuestionsRequestData(string chapterID)
            {
                chapter_id = chapterID;
            }
        }
        
        public class GetQuestionsResponse : ResponseBase
        {
            public List<GetQuestionsResponseData> ResponseData;
        }

        public class GetQuestionsResponseData
        {
            public string id;
            public string title;
            public List<Answer> answers;
        }
        
        public class Answer
        {
            public string title;
            public bool option_status;
        }
    }
}