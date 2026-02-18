using DebugUtils;
using Newtonsoft.Json;
using RestAPI;
using System;
using UnityEngine;
using static BibleGame.API.LeaderBoardAPI;


namespace BibleGame
{
    namespace API
    {
        public class VersionAPI : ApiBase
        {
            public delegate void VersionCallback(bool success, VersionResponse response = null);

            public static void GetVersion(VersionCallback callback)
            {
                var url = $"{ServiceURL.baseURL}{ServiceURL.version}";

                DevDebug.Log($"Get the version: {url}", DebugColor.Cyan);

                WebRequestGet(url, (url, success, adata) =>
                {
                    DevDebug.Log($"Response of the version : {adata.ToString()}", DebugColor.Orange);

                    if(!success)
                    {
                        callback?.Invoke(false, null);
                        return;
                    }

                    var data = JsonConvert.DeserializeObject<VersionResponse>(adata.ToString());

                    if(data == null)
                    {
                        callback?.Invoke(false, null);
                        return;
                    }

                    callback?.Invoke(true, data);
                });
            }
            #region RESPONSE
            public class VersionResponse : ResponseBase
            {
                public VersionData ResponseData;
            }

            [Serializable]
            public class VersionData
            {
                public string _id;
                public string andriod_minimum_version;
                public string andriod_latest_version;
                public string ios_minimum_version;
                public string ios_latest_version;
            }

            #endregion
        }
    }
}
