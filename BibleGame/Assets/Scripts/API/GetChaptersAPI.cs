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
        public class ChapterAPI : ApiBase
        {

            public delegate void GetChapterListCallback(bool success, GetChaptersResponse response = null);
            public delegate void GetChapterDetailCallback(bool success, ChapterDetailResponse response = null);



            public static void Get(GetChapterListCallback callback,string bibleid,string bookid)
            {
                var url = $"{ServiceURL.baseURL}{ServiceURL.getChapters}?bible_id={bibleid}&book_id={bookid}";

                WebRequestGet(url, (url, success, adata) =>
                {
                    if(!success)
                    {
                        Debug.LogError("No success in getting the chapters");
                        callback?.Invoke(false, null);
                        return;
                    }

                    try
                    {
                        var data = JsonConvert.DeserializeObject<GetChaptersResponse>(adata.ToString());

                        List<ChapterData> chapterlist = data.ResponseData.data;
                        ChapterData intro = chapterlist.Find(x => x.number == "intro");

                        chapterlist.Remove(intro);
                        data.ResponseData.data = chapterlist;

                        callback?.Invoke(success, data);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error in getting the chapters: {ex.Message}");
                        callback?.Invoke(false, null);
                    }
                });
            }

            public static void GetDetail(GetChapterDetailCallback callback , string bibleId,string chapterId)
            {
                var url = $"{ServiceURL.baseURL}{ServiceURL.getChapterDetail}?bible_id={bibleId}&chapter_id={chapterId}";

                WebRequestGet(url, (url, success, adata) =>
                {
                    if (!success)
                    {
                        Debug.LogError($"No success in getting the chapter detail {adata.ToString()}");
                        callback?.Invoke(false, null);
                        return;
                    }

                    try
                    {
                        var data = JsonConvert.DeserializeObject<ChapterDetailResponse>(adata.ToString());
                        callback?.Invoke(success, data);
                    }
                    catch(Exception ex)
                    {
                        Debug.LogError($"Error in getting the chapter detail: {ex.Message}");
                        callback?.Invoke(false , null);
                    }

                });
            }

          
        }


        #region RESPONSE
        public class GetChaptersResponse : ResponseBase
        {
            public ChapterDataResponse ResponseData;
        }

        [Serializable]
        public class ChapterDataResponse
        {
            public List<ChapterData> data;
        }

        [Serializable]
        public class ChapterData
        {
            public string id;
            public string bibleId;
            public string bookId;
            public string number;
            public string reference;
        }

        public class ChapterDetailResponse: ResponseBase
        {
            public ChapterDResponseData ResponseData;
        }

        [Serializable]
        public class ChapterDResponseData
        {
            public ChapterDetail data;
        }

        [Serializable]
        public class ChapterDetail
        {
            public string id;
            public string bookId;
            public string number;
            public string content;
        }

        #endregion
    }
}