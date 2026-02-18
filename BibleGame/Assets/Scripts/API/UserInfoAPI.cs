using DebugUtils;
using Newtonsoft.Json;
using RestAPI;
using System;
using UnityEngine;
using static BibleGame.API.VersionAPI;


namespace BibleGame
{
    namespace API
    {
        public class UserInfoAPI : ApiBase
        {
            public delegate void UserInfoCallback(bool success, UserInfoResponse response = null);

            public static void Get(UserInfoCallback callback, string id)
            {
                var url = $"{ServiceURL.baseURL}{ServiceURL.userInfo}";

                var json =  JsonConvert.SerializeObject(new { user_id = id });

                DevDebug.Log($"Get the info for {json} :: {url} ", DebugColor.Cyan);

                WebRequest(url, json, (url, success, adata) =>
                {
                    DevDebug.Log($"Response of the userInfo : {adata.ToString()}", DebugColor.Orange);

                    if (!success)
                    {
                        callback?.Invoke(false, null);
                        return;
                    }

                    var data = JsonConvert.DeserializeObject<UserInfoResponse>(adata.ToString());

                    if (data == null)
                    {
                        callback?.Invoke(false, null);
                        return;
                    }

                    callback?.Invoke(true, data);
                });
            }

            #region RESPONSE
            public class UserInfoResponse : ResponseBase
            {
                public userInfoData ResponseData;
            }

            [Serializable]
            public class userInfoData
            {
                public string user_id;
                public string user_name;
                public string profile_pic;
                public int total_game_streak;
                public int total_coins_earned;
                public int levels_finished;
            }
            #endregion
        }
    }
}