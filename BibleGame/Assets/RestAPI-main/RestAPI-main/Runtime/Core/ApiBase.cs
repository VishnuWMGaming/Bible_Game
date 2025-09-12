using System.Collections.Generic;
using UnityEngine;

namespace RestAPI
{
    public abstract  class ApiBase
    {

        private static string AuthKey = "authorization";
        private static KeyValuePair<string, string> AuthKeyPair = new KeyValuePair<string, string>(AuthKey,"");
        
        public static void SetAuthToken(string token)
        {
            AuthKeyPair = new KeyValuePair<string, string>(AuthKey,"bearer "+token);
        }
        
        public static void SetAuthToken(string token , string bearer)
        {
            AuthKeyPair = new KeyValuePair<string, string>(AuthKey,bearer +token);
        }
        
        protected static void WebRequest(string serviceUrl, string jsonData, WebHelpers.CallbackGet callback)
        {

            if (string.IsNullOrEmpty(jsonData))
            {
                jsonData = "{}";
            }

            var bytesContent = System.Text.Encoding.UTF8.GetBytes(jsonData);
           // LogSystem.LogEvent("[][] requesting url {0}, requestParams {1}", url, jsonData);
            WebHelpers.Instance.Post<string>(serviceUrl, bytesContent, "application/json",  callback,AuthKeyPair);
        }

        protected static void WebRequest(string serviceUrl, WWWForm formData, WebHelpers.CallbackGet callback)
        {
     
            // LogSystem.LogEvent("[][] requesting url {0}, requestParams {1}", url, formData);
            WebHelpers.Instance.Post<string>(serviceUrl, formData, "application/json", callback , AuthKeyPair);
        }

        protected static void WebRequestGet(string serviceUrl, WebHelpers.CallbackGet callback)
        {
            // var url = BaseURL + serviceName;

            Debug.LogFormat("[][] requesting url {0}", serviceUrl);
            WebHelpers.Instance.Get<string>(serviceUrl, callback  ,AuthKeyPair);
        }
        
        protected static void WebRequestGet(string serviceUrl, string jsonData, WebHelpers.CallbackGet callback)
        {
            // var url = BaseURL + serviceName;
            if (string.IsNullOrEmpty(jsonData))
            {
                jsonData = "{}";
            }

            var bytesContent = System.Text.Encoding.UTF8.GetBytes(jsonData);
            
            
            Debug.LogFormat("[][] requesting url {0}", serviceUrl);
            WebHelpers.Instance.Get<string>(serviceUrl,bytesContent, "application/json" , callback);
        }
        
        
        protected static void WebRequestPatch(string serviceUrl, string jsonData, WebHelpers.CallbackPatch callback)
        {

            if (string.IsNullOrEmpty(jsonData))
            {
                jsonData = "{}";
            }

            var bytesContent = System.Text.Encoding.UTF8.GetBytes(jsonData);
            // LogSystem.LogEvent("[][] requesting url {0}, requestParams {1}", url, jsonData);
            WebHelpers.Instance.Patch<string>(serviceUrl, bytesContent, "application/json",  callback, AuthKeyPair);
        }

        
        protected static void WebRequestGetImage(string serviceUrl, WebHelpers.CallbackGet callback)
        {
            // var url = BaseURL + serviceName;

            Debug.LogFormat("[][] requesting url {0}", serviceUrl);
            WebHelpers.Instance.Get<Texture2D>(serviceUrl, callback , AuthKeyPair);
        }
        
    }
}