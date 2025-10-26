using DebugUtils;
using Newtonsoft.Json;
using RestAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;



namespace BibleGame
{
    namespace API
    {
        public  class LeaderBoardAPI : ApiBase
        {
            public delegate void LeaderBoardCallback(bool success, LeaderBoardResponse response = null);


            public static void GetData(LeaderBoardCallback callback)
            {
                var url = $"{ServiceURL.baseURL}{ServiceURL.getLeaderBoard}";

                DevDebug.Log($"Get the leaderBoard: {url}", DebugColor.Cyan);

                WebRequestGet(url, (url, success, adata) =>
                {
                    DevDebug.Log($"Response of the leaderboard... {adata.ToString()}", DebugColor.Orange);

                    if(!success)
                    {
                        callback?.Invoke(false, null);
                        Debug.LogError("No success in getting the leaderboard");
                        return;
                    }

                    var data = JsonConvert.DeserializeObject<LeaderBoardResponse>(adata.ToString());

                    if(data == null)
                    {
                        callback?.Invoke(false, null);
                        Debug.LogError("Not able to deserialise in getting the leaderboard");
                        return;
                    }

                    callback?.Invoke(true, data);
                });
            }


            #region RESPONSE

            public class LeaderBoardResponse : ResponseBase
            {
                public List<RankUser> ResponseData;
            }

            [Serializable]
            public class RankUser
            {
                public string user_id;
                public int total_game_streak;
                public int total_streak_level;
                public int coin_earn;
                public string user_name;
                public string profile_pic;
                public float score;
                public int rank;
            }

            #endregion

        }
    }
}

