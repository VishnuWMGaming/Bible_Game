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
        public class TranslateAPI : ApiBase
        {
            public delegate void TranslateaCallback(bool success, TransResponse response = null);


            public static void Translate(TranslateaCallback callback, TransInput input)
            {
                string url = $"{ServiceURL.baseURL}{ServiceURL.getTrans}";

                string jsonData = JsonConvert.SerializeObject(input);
               // DebugUtils.DevDebug.Log($"translate  :{url} : {jsonData}",DebugColor.Magenta);

                WebRequest(url, jsonData, (url, success, adata) =>
               {
                  // Debug.Log($"<color=#FFA500> trramnslate response : {adata.ToString()}</color>");

                   if (!success)
                   {
                       Debug.LogError("No success in get translation");
                       callback?.Invoke(false, null);
                       return;
                   }

                   var data = JsonConvert.DeserializeObject<TransResponse>(adata.ToString());

                   if (data == null)
                   {
                       Debug.LogError("No success in get translation");
                       callback?.Invoke(false, null);
                       return;
                   }

                   callback?.Invoke(true, data);
               });

            }
        }


        #region  REQUEST
        [Serializable]
        public class TransInput
        {
            public string text;
            public string language;
        }


        #endregion

        #region  RESPONSE
         public class TransResponse:ResponseBase
        {
            public string ResponseData;
        }


        #endregion
    }
}
