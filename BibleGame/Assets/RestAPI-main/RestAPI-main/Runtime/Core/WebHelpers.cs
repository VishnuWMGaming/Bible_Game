using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RestAPI
{
    public class WebHelpers : MonoBehaviour
    {
        // Delegates
        //Call back for APIs Response
        public delegate void CallbackGet(string aURL, bool aSuccess, object aData);
        public delegate void CallbackPatch(string aURL, long responseCode, bool aSuccess, object aData);
        //public delegate void CallbackPost(string aURL, bool aSuccess, object aData);
        // The return data types we support
        private readonly List<System.Type> supportedTypes = new List<System.Type>
        {
            typeof(string),
            typeof(Texture2D),
            typeof(byte[])
        };

        public static WebHelpers Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this; 
            }
            else if (Instance != this)
            {
                    Destroy(this);
            }

        }


        #region Public Methods

        /// <summary>
        /// Uses a HTTP GET request with the specified URL.
        /// </summary>
        /// <typeparam name="T">Can be any of the supported return types - string, byte[] or Texture2D</typeparam>
        /// <param name="aURL">Request URL</param>
        /// <param name="aCallback">Called when the request is complete</param>
        /// <param name="isHeaderRequired"></param>
        public void Get<T>(string aURL, CallbackGet aCallback,  KeyValuePair<string,string> authToken, bool isHeaderRequired = true)
        {
            // sanity - checks for supported types
            var dataType = typeof(T);
            if (!supportedTypes.Contains(dataType))
            {
                //Unsupported data type go back
                Debug.LogErrorFormat("WEB HELPERS: POST: {0}: Unsupported data type => {1}", aURL, dataType);

                return;
            }

            // create the request for valid data type request
            var req = new UnityWebRequest(aURL);
            req.method = UnityWebRequest.kHttpVerbGET;
     
            req.SetRequestHeader(authToken.Key,authToken.Value);  
            


            //var uploadHandler = new UploadHandlerRaw(aContent);
            //uploadHandler.contentType = aContentType;
            //req.uploadHandler = uploadHandler;
            //req.disposeUploadHandlerOnDispose = true;

            // select the right handler based on supported types
            DownloadHandler dataHandler;
            // textures
            if (dataType == typeof(Texture2D))
            {
                dataHandler = new DownloadHandlerTexture();
            }
            // default
            else
            {
                dataHandler = new DownloadHandlerBuffer();
            }
            req.downloadHandler = dataHandler;
            req.disposeDownloadHandlerOnDispose = true;
            StartCoroutine(_getRequest<T>(req, aCallback));
        }

        
        public void Get<T>(string aURL, byte[] aContent , string aContentType  , CallbackGet aCallback, bool isHeaderRequired = true)
        {
            // sanity - checks for supported types
            var dataType = typeof(T);
            if (!supportedTypes.Contains(dataType))
            {
                //Unsupported data type go back
                Debug.LogErrorFormat("WEB HELPERS: POST: {0}: Unsupported data type => {1}", aURL, dataType);

                return;
            }

            // create the request for valid data type request
            var req = new UnityWebRequest(aURL);
            req.method = UnityWebRequest.kHttpVerbGET;
     
            
            


            var uploadHandler = new UploadHandlerRaw(aContent);
            uploadHandler.contentType = aContentType;
            req.uploadHandler = uploadHandler;
            req.disposeUploadHandlerOnDispose = true;

            // select the right handler based on supported types
            DownloadHandler dataHandler;
            // textures
            if (dataType == typeof(Texture2D))
            {
                dataHandler = new DownloadHandlerTexture();
            }
            // default
            else
            {
                dataHandler = new DownloadHandlerBuffer();
            }
            req.downloadHandler = dataHandler;
            req.disposeDownloadHandlerOnDispose = true;
            StartCoroutine(_getRequest<T>(req, aCallback));
        }
        

        public void Post<T>(string aURL, byte[] aContent, string aContentType, CallbackGet aCallback, KeyValuePair<string,string> authToken)
        {
            // sanity - checks for supported types
            var dataType = typeof(T);
            if (!supportedTypes.Contains(dataType))
            {
                Debug.LogErrorFormat("WEB HELPERS: POST: {0}: Unsupported data type => {1}", aURL, dataType);
                return;
            }

            // create the request
            var req = new UnityWebRequest(aURL);
            req.method = UnityWebRequest.kHttpVerbPOST;

          req.SetRequestHeader(authToken.Key,authToken.Value);  
            
            
#if AUTHVALUE
				if (!string.IsNullOrEmpty(GlobalData.UserToken))
			{
				req.SetRequestHeader("token", GlobalData.UserToken);
			}
#endif


            var uploadHandler = new UploadHandlerRaw(aContent);
            uploadHandler.contentType = aContentType;
            req.uploadHandler = uploadHandler;
            req.disposeUploadHandlerOnDispose = true;


            // select the right handler based on supported types
            DownloadHandler dataHandler;
            // textures
            if (dataType == typeof(Texture2D))
            {
                dataHandler = new DownloadHandlerTexture();
            }
            // default
            else
            {
                dataHandler = new DownloadHandlerBuffer();
            }

            req.downloadHandler = dataHandler;
            req.disposeDownloadHandlerOnDispose = true;

            //Debug.LogFormat("WEB HELPERS: POST: {0}: Fetching as {1}", aURL, dataType.ToString());

            // Go Ninja Go!
            StartCoroutine(_postRequest<T>(req, aCallback));
        }


        /// <summary>
        /// Uses a HTTP POST request with the specified URL.
        /// </summary>
        /// <typeparam name="T">Can be any of the supported return types - string, byte[] or Texture2D</typeparam>
        /// <param name="aURL">Request URL</param>
        /// <param name="aContent">Content to upload as the body of the request</param>
        /// <param name="aContentType">Content type as per HTTP specs</param>
        /// <param name="aCallback">Called when the request is complete</param>
        public void Post<T>(string aURL, WWWForm aContent, string aContentType, CallbackGet aCallback, KeyValuePair<string,string> authToken)
        {
            // sanity - checks for supported types
            var dataType = typeof(T);
            if (!supportedTypes.Contains(dataType))
            {
                //Debug.LogErrorFormat("WEB HELPERS: POST: {0}: Unsupported data type => {1}", aURL, dataType.ToString());
                return;
            }

            // create the request
            var req = UnityWebRequest.Post(aURL, aContent);
            req.method = UnityWebRequest.kHttpVerbPOST;

            req.SetRequestHeader(authToken.Key,authToken.Value);  
            
            
            
            // upload handler sends our body
            //var uploadHandler = new UploadHandler(aContent);
            //uploadHandler.contentType = aContentType;
            //req.uploadHandler = uploadHandler;
            //req.disposeUploadHandlerOnDispose = true;
            // select the right handler based on supported types
            DownloadHandler dataHandler;
            // textures
            if (dataType == typeof(Texture2D))
            {
                dataHandler = new DownloadHandlerTexture();
            }
            // default
            else
            {
                dataHandler = new DownloadHandlerBuffer();
            }

            req.downloadHandler = dataHandler;
            req.disposeDownloadHandlerOnDispose = true;

            //TODO might need it
            //Debug.LogFormat("WEB HELPERS: POST: {0}: Fetching as {1}", aURL, dataType.ToString());

            // Go Ninja Go!
            StartCoroutine(_postRequest<T>(req, aCallback, aContent));
        }

        public void Patch<T>(string aURL, byte[] aContent, string aContentType, CallbackPatch aCallback, KeyValuePair<string,string> authToken)
        {
            // sanity - checks for supported types
            var dataType = typeof(T);
            if (!supportedTypes.Contains(dataType))
            {
                Debug.LogErrorFormat("WEB HELPERS: POST: {0}: Unsupported data type => {1}", aURL, dataType);
                return;
            }

            // create the request
            var req = new UnityWebRequest(aURL);
            req.method = "PATCH";

            req.SetRequestHeader(authToken.Key,authToken.Value);  
            
            
#if AUTHVALUE
				if (!string.IsNullOrEmpty(GlobalData.UserToken))
			{
				req.SetRequestHeader("token", GlobalData.UserToken);
			}
#endif


            var uploadHandler = new UploadHandlerRaw(aContent);
            uploadHandler.contentType = aContentType;
            req.uploadHandler = uploadHandler;
            req.disposeUploadHandlerOnDispose = true;


            // select the right handler based on supported types
            DownloadHandler dataHandler;
            // textures
            if (dataType == typeof(Texture2D))
            {
                dataHandler = new DownloadHandlerTexture();
            }
            // default
            else
            {
                dataHandler = new DownloadHandlerBuffer();
            }

            req.downloadHandler = dataHandler;
            req.disposeDownloadHandlerOnDispose = true;

            //Debug.LogFormat("WEB HELPERS: POST: {0}: Fetching as {1}", aURL, dataType.ToString());

            // Go Ninja Go!
            StartCoroutine(_patchRequest<T>(req, aCallback));
        }
        
        
        #endregion Public Methods


        #region Private Coroutines

        
        private static IEnumerator _patchRequest<T>(UnityWebRequest aRequest, CallbackPatch aCallback)
        {
            // send off the request and wait
            yield return aRequest.SendWebRequest();

            // handle the results
#pragma warning disable CS0618
            if (aRequest.isNetworkError || aRequest.isHttpError)
#pragma warning restore CS0618
            {
                // something went wrong!
                if (aRequest.responseCode == 401)
                {
                    //TODO: Take user to login as session Expired
                    //MGMManager.Instance.AskUserLogin();
                }

                //Debug.LogErrorFormat("WEB HELPERS: POST: {0}: Failed => {1} (HTTP {2})", aRequest.url, aRequest.error, aRequest.responseCode);
                aCallback(aRequest.url,  aRequest.responseCode, false, aRequest.error);
            }
            else
            {
                //Debug.LogFormat("WEB HELPERS: POST: {0}: Fetched => {1} bytes", aRequest.url, aRequest.downloadHandler.data.Length);

                // which data type was specified?
                var dataType = typeof(T);
                if (dataType == typeof(Texture2D))
                {
                    var dataHandler = (DownloadHandlerTexture) aRequest.downloadHandler;
                    aCallback(aRequest.url, aRequest.responseCode, true, dataHandler.texture);
                }
                else if (dataType == typeof(string))
                {
                    var dataHandler = (DownloadHandlerBuffer) aRequest.downloadHandler;
                    aCallback(aRequest.url,  aRequest.responseCode,true, dataHandler.text);
                }
                else if (dataType == typeof(byte[]))
                {
                    var dataHandler = (DownloadHandlerBuffer) aRequest.downloadHandler;
                    aCallback(aRequest.url, aRequest.responseCode, true, dataHandler.data);
                }
            }

            // be polite and get rid of the request object to avoid leaks
            aRequest.Dispose();
        }
        
        private static IEnumerator _postRequest<T>(UnityWebRequest aRequest, CallbackGet aCallback)
        {
            // send off the request and wait
            yield return aRequest.SendWebRequest();

            // handle the results
#pragma warning disable CS0618
            if (aRequest.isNetworkError || aRequest.isHttpError)
#pragma warning restore CS0618
            {
                // something went wrong!
                if (aRequest.responseCode == 401)
                {
                    //TODO: Take user to login as session Expired
                    //MGMManager.Instance.AskUserLogin();
                }

                //Debug.LogErrorFormat("WEB HELPERS: POST: {0}: Failed => {1} (HTTP {2})", aRequest.url, aRequest.error, aRequest.responseCode);
                //aCallback(aRequest.url, false, aRequest.error);
                var dataHandler = (DownloadHandlerBuffer) aRequest.downloadHandler;
                aCallback(aRequest.url, false, dataHandler.text);
            }
            else
            {
                //Debug.LogFormat("WEB HELPERS: POST: {0}: Fetched => {1} bytes", aRequest.url, aRequest.downloadHandler.data.Length);

                // which data type was specified?
                var dataType = typeof(T);
                if (dataType == typeof(Texture2D))
                {
                    var dataHandler = (DownloadHandlerTexture) aRequest.downloadHandler;
                    aCallback(aRequest.url, true, dataHandler.texture);
                }
                else if (dataType == typeof(string))
                {
                    var dataHandler = (DownloadHandlerBuffer) aRequest.downloadHandler;
                    aCallback(aRequest.url, true, dataHandler.text);
                }
                else if (dataType == typeof(byte[]))
                {
                    var dataHandler = (DownloadHandlerBuffer) aRequest.downloadHandler;
                    aCallback(aRequest.url, true, dataHandler.data);
                }
            }

            // be polite and get rid of the request object to avoid leaks
            aRequest.Dispose();
        }


        private static IEnumerator _getRequest<T>(UnityWebRequest aRequest, CallbackGet aCallback)
        {
            // send off the request and wait
            yield return aRequest.SendWebRequest();

            // handle the results
#pragma warning disable CS0618
            if (aRequest.isNetworkError || aRequest.isHttpError)
#pragma warning restore CS0618
            {
                // something went wrong!

                if (aRequest.responseCode == 401)
                {
                    //TODO: Take user to login as session Expired
                    //MGMManager.Instance.AskUserLogin();
                }

                //Debug.LogErrorFormat("WEB HELPERS: GET: {0}: Failed => {1} (HTTP {2})", aRequest.url, aRequest.error, aRequest.responseCode);
                aCallback(aRequest.url, false, aRequest.error);
            }
            else
            {
                //Debug.LogFormat("WEB HELPERS: GET: {0}: Fetched => {1} bytes", aRequest.url, aRequest.downloadHandler.data.Length);

                // which data type was specified?
                var dataType = typeof(T);
                if (dataType == typeof(Texture2D))
                {
                    var dataHandler = (DownloadHandlerTexture) aRequest.downloadHandler;
                    aCallback(aRequest.url, true, dataHandler.texture);
                }
                else if (dataType == typeof(string))
                {
                    var dataHandler = (DownloadHandlerBuffer) aRequest.downloadHandler;
                    aCallback(aRequest.url, true, dataHandler.text);
                }
                else if (dataType == typeof(byte[]))
                {
                    var dataHandler = (DownloadHandlerBuffer) aRequest.downloadHandler;
                    aCallback(aRequest.url, true, dataHandler.data);
                }
            }

            // be polite and get rid of the request object to avoid leaks
            aRequest.Dispose();
        }


        private static IEnumerator _postRequest<T>(UnityWebRequest aRequest, CallbackGet aCallback, WWWForm aContent)
        {
            // send off the request and wait
            yield return aRequest.SendWebRequest();

            // handle the results
#pragma warning disable CS0618
            if (aRequest.isNetworkError || aRequest.isHttpError)
#pragma warning restore CS0618
            {
                // something went wrong!
                //TODO might need it
                //              Debug.LogErrorFormat("WEB HELPERS: POST: {0}: Failed => {1} (HTTP {2})", aRequest.url, aRequest.error, aRequest.responseCode);
               // aCallback(aRequest.url, false, aRequest.error);
               var dataHandler = (DownloadHandlerBuffer) aRequest.downloadHandler;
               aCallback(aRequest.url, false, dataHandler.text);
            }
            else
            {
                //TODO might need it
                // yay!
                //              Debug.LogFormat("WEB HELPERS: POST: {0}: Fetched => {1} bytes", aRequest.url, aRequest.downloadHandler.data.Length);

                // which data type was specified?
                var dataType = typeof(T);
                if (dataType == typeof(Texture2D))
                {
                    var dataHandler = (DownloadHandlerTexture) aRequest.downloadHandler;
                    aCallback(aRequest.url, true, dataHandler.texture);
                }
                else if (dataType == typeof(string))
                {
                    var dataHandler = (DownloadHandlerBuffer) aRequest.downloadHandler;
                    aCallback(aRequest.url, true, dataHandler.text);
                }
                else if (dataType == typeof(byte[]))
                {
                    var dataHandler = (DownloadHandlerBuffer) aRequest.downloadHandler;
                    aCallback(aRequest.url, true, dataHandler.data);
                }
            }

            // be polite and get rid of the request object to avoid leaks
            aRequest.Dispose();
        }

        #endregion
    }
}