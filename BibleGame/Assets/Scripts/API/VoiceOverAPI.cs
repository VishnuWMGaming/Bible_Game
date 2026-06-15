using DebugUtils;
using Newtonsoft.Json;
using RestAPI;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using static BibleGame.API.UserInfoAPI;
using static Unity.Collections.Unicode;

namespace BibleGame
{
    namespace API
    {
        public class VoiceOverAPI : ApiBase
        {
            public delegate void VoiceOverCallback(bool success, AudioClip clip = null);


            public static void Get(MonoBehaviour runner, VoiceOverCallback callback, string text,Language language)
            {
                var url = $"{ServiceURL.voiceURL}";


                string code = language switch
                { 
                    Language.English => "en-US",
                    Language.Spanish => "es-ES",
                    Language.Portuguese => "pt-BR",
                    Language.Korean => "ko-KR",
                    Language.French => "fr-FR",
                    Language.Chinese => "cmn-CN",
                    Language.Swahili => "en-US",
                    Language.Kreyol => "ht-HT",
                    Language.Hindi => "hi-IN"
                };

                string voice = language switch
                {
                    Language.English => "en-US-Neural2-J",
                    Language.Spanish => "es-ES-Neural2-B",
                    Language.Portuguese => "pt-BR-Neural2-B",
                    Language.Korean => "ko-KR-Neural2-B",
                    Language.French => "fr-FR-Neural2-B",
                    Language.Chinese => "cmn-CN-Standard-B",
                    Language.Swahili => "en-US-Neural2-J",
                    Language.Kreyol => "Puck",
                    Language.Hindi => "hi-IN-Neural2-B"
                };

                GetVoiceIput iput = new GetVoiceIput
                {
                    text = text,
                    gender = "male",
                    languageCode = code,
                    voiceName = voice
                };
             
                var json = JsonConvert.SerializeObject(iput);

                DevDebug.Log($"Get the voice for {json} :: {url} ", DebugColor.Cyan);

                runner.StartCoroutine(GetVoiceAudio(url, json, callback));
            }

            private static bool IsBase64String(string s)
            {
                if (string.IsNullOrWhiteSpace(s))
                    return false;

                s = s.Trim();

                return (s.Length % 4 == 0) &&
                       System.Text.RegularExpressions.Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,2}$");
            }

            private static IEnumerator GetVoiceAudio(string url, string json, VoiceOverCallback callback)
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

                using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
                {
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = new DownloadHandlerBuffer();

                    request.SetRequestHeader("Content-Type", "application/json");
                    request.SetRequestHeader("Accept", "audio/mpeg"); // 🔥 important

                    request.disposeUploadHandlerOnDispose = true;
                    request.disposeDownloadHandlerOnDispose = true;

                    yield return request.SendWebRequest();

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError("Audio Request Failed: " + request.error);
                        callback?.Invoke(false, null);
                    }
                    else
                    {
                        byte[] audioBytes = request.downloadHandler.data;

                        if (audioBytes == null || audioBytes.Length == 0)
                        {
                            Debug.LogError("Downloaded audio is empty");
                            callback?.Invoke(false, null);
                            yield break;
                        }

                        string path = Path.Combine(Application.persistentDataPath, "voice.mp3");
                        File.WriteAllBytes(path, audioBytes);

                        yield return LoadAudio(path, callback);
                    }
                }
            }

            private static IEnumerator LoadAudio(string path, VoiceOverCallback callback)
            {
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, UnityEngine.AudioType.UNKNOWN))
                {
                    yield return www.SendWebRequest();

                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError(www.error);
                        callback?.Invoke(false, null);
                    }
                    else
                    {
                        AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                        callback?.Invoke(true, clip);
                    }
                }
            }

        }

        #region REQUEST
        [Serializable]
        public class GetVoiceIput
        {
            public string text;
            public string gender;
            public string languageCode;
            public string voiceName;
        }

        #endregion

        #region RESPONSE
        public class GetVoiceResponse : ResponseBase
        {

        }


        #endregion
    }
}