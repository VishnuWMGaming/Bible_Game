using Newtonsoft.Json;
using RestAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;


namespace BibleGame
{
    namespace API
    {
        public class HintAPI : ApiBase
        {
            public delegate void GetFreeHintCallback (bool success, GetFreeHintResponse response);
            public delegate void DeductFreeHintCallback(bool success);

            public static void GetFreeHint(GetFreeHintCallback callback)
            {
                var url = $"{ServiceURL.baseURL}{ServiceURL.getFreeHint}";

                DebugUtils.DevDebug.Log($"Get the free Hint : {url}", DebugColor.Cyan);

                WebRequestGet(url, (aURL, aSuccess, aData) =>
                {
                    DebugUtils.DevDebug.Log($"Response of the get free hints: {aData.ToString()}", DebugColor.Orange);

                    if(!aSuccess)
                    {
                        callback?.Invoke(false, null);
                        Debug.LogError("No Success at getting free hints");
                        return;
                    }

                    var data = JsonConvert.DeserializeObject<GetFreeHintResponse>(aData.ToString());

                    if(data == null)
                    {
                        callback?.Invoke(false, null);
                        Debug.LogError("No data at the getting the free hints");
                        return;
                    }

                    callback?.Invoke(true, data);

                });
            }


            public static void DeductFreeHint(DeductFreeHintCallback callback)
            {
                var url = $"{ServiceURL.baseURL}{ServiceURL.deductFreeHint}";

                DebugUtils.DevDebug.Log($"Deduct free Hint : {url}", DebugColor.Cyan);

                WebRequestGet(url, (aURL, aSuccess, aData) =>
                {
                    DebugUtils.DevDebug.Log($"Response of Deduct hints: {aData.ToString()}", DebugColor.Orange);

                    if (!aSuccess)
                    {
                        callback?.Invoke(false);
                        Debug.LogError("No Success at Deduct free hints");
                        return;
                    }

                    var data = JsonConvert.DeserializeObject<DedcutHintResponse>(aData.ToString());

                    if (data == null)
                    {
                        callback?.Invoke(false);
                        Debug.LogError("No data at deduct free hints");
                        return;
                    }

                    callback?.Invoke(true);
                });
            }
        }


        #region RESPONSE

        public class DedcutHintResponse : ResponseBase
        {
          
        }


        public class GetFreeHintResponse : ResponseBase
        {
            public FreeHintData ResponseData;
        }

        [Serializable]
        public class FreeHintData
        {
            public string user_id;
            public string name;
            public int freeHint;
        }
      
        #endregion
    }
}

