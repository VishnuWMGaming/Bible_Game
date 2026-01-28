using BibleGame.Data;
using DebugUtils;
using Newtonsoft.Json;
using RestAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BibleGame
{
    namespace API
    {
        public class GetBiblesAPI : ApiBase
        {
            public delegate void GetBiblesCallback(bool success, GetBibleResponse response = null);
            public delegate void GetBiblesDetailCallback(bool success, GetBibleDetailResponse response = null);
            public delegate void GetBookListCallback(bool success, GetBookListResponse response = null);

            public static List<BibleData> mBibleDatas = new List<BibleData>();
            public static List<BookData> mBookDatas = new List<BookData>();

            public static void Get(GetBiblesCallback callback)
            {
                var url = ServiceURL.baseURL + ServiceURL.getBible;

                WebRequestGet(url, (url, success, adata) =>
                {
                    if (!success)
                    {
                        Debug.LogError("No Success in get bible");
                        callback?.Invoke(false, null);

                        return;
                    }

                    try
                    {
                        Debug.Log($"<color=magenta> {adata.ToString()} </color>");

                        var data = JsonConvert.DeserializeObject<GetBibleResponse>(adata.ToString());

                        mBibleDatas = data.ResponseData;
                        callback?.Invoke(success, data);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Get bible {e.Message}");
                        callback?.Invoke(false, null);
                    }
                });
            }

            public static void GetDetail(GetBiblesDetailCallback callback, string id)
            {
                var url = $"{ServiceURL.baseURL}{ServiceURL.getBibleDetail}?bible_id={id}";

                DevDebug.Log($"Get bibles: url =>{url}", DebugColor.Cyan);

                WebRequestGet(url, (url, success, adata) =>
                {
                    DevDebug.Log($"Get bibles response  =>{adata.ToString()}", DebugColor.Orange);

                    if (!success)
                    {
                        Debug.LogError("No Success in get bible detail");
                        callback?.Invoke(false, null);

                        return;
                    }

                    try
                    {
                        var data = JsonConvert.DeserializeObject<GetBibleDetailResponse>(adata.ToString());

                        callback?.Invoke(success, data);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Get bible {e.Message}");
                        callback?.Invoke(false, null);
                    }

                });
            }

            public static void GetBookList(GetBookListCallback callback, string id)
            {
                var url = $"{ServiceURL.baseURL}{ServiceURL.getBookDetail}?bible_id={id}&language={LanguageController.Instance.GetLangStringVal(AppData.mLanguage)}";

                Debug.Log($"<color=#FFA500> get book list : url => {url}</color>");

                WebRequestGet(url, (url, success, adata) =>
                {
                    Debug.Log($"<color=magenta> book list data: {adata.ToString()}</color>");

                    if (!success)
                    {
                        Debug.LogError("No Success in get book detail");
                        callback?.Invoke(false, null);

                        return;
                    }




                    try
                    {
                        var data = JsonConvert.DeserializeObject<GetBookListResponse>(adata.ToString());

                        callback?.Invoke(success, data);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Get book list : {e.Message}");
                        callback?.Invoke(false, null);
                    }
                });
            }
             
            #region RESPONSE
            public class GetBibleResponse : ResponseBase
            {
                public List<BibleData> ResponseData;
            }

            [Serializable]
            public class BibleData
            {
                public string id;
                public string name;
                public string bible_id;
                public string language;
            }

            public class GetBibleDetailResponse : ResponseBase
            {
                public GetBibleDatR ResponseData;
            }

            [Serializable]
            public class GetBibleDatR
            {
                public GetBibleData data;
            }

            [Serializable]
            public class GetBibleData
            {
                public string nameLocal;
            }

            public class GetBookListResponse : ResponseBase
            {
                public List<BookData>  ResponseData;
            }

            [Serializable]
            public class BookList
            {
                public List<BookData> data;
            }

            [Serializable]
            public class BookData
            {
                public string id;
                public string bibleId;
                public string abbreviation;
                public string name;
                public string nameLong;
            }
            #endregion
        }
    }
}