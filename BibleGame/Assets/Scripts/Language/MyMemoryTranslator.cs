using Lean.Localization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class MyMemoryTranslator : MonoBehaviour
{
    [Serializable]
    private class MyMemoryResponse
    {
        [Serializable]
        public class ResponseDataClass
        {
            public string translatedText;
        }

        public ResponseDataClass responseData;

        [Serializable]
        public class Match
        {
            public string source; // detected language code
        }

        public Match[] matches;
    }

    /// <summary>
    /// Translate a phrase via MyMemory API and store in LeanLocalization.
    /// </summary>
    public IEnumerator TranslateAndStore(string phraseKey, string sourceLang, string targetLang, Action<string> finish = null)
    {
        // 1️⃣ Get source text from Lean (or fallback to key)
        string sourceText = LeanLocalization.GetTranslationText(phraseKey, sourceLang.ToLower(), false);
        if (string.IsNullOrEmpty(sourceText) || sourceText == sourceLang)
        {
            sourceText = phraseKey;

            // 2️⃣ Build MyMemory GET request
            string url = $"https://api.mymemory.translated.net/get?q={UnityWebRequest.EscapeURL(sourceText)}&langpair={sourceLang.ToLower()}|{targetLang.ToLower()}";

            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string json = www.downloadHandler.text;

                    // Parse response
                    var response = JsonUtility.FromJson<MyMemoryResponse>(json);

                    if (response != null && response.responseData != null)
                    {
                        string translated = response.responseData.translatedText;

                        // Save in LeanLocalization
                        var loc = LeanLocalization.GetOrCreateInstance();
                        var phrase = loc.AddPhrase(phraseKey);
                        phrase.AddEntry(targetLang, translated);

                        Debug.Log($"✅ Auto-translated '{phraseKey}' → {targetLang}: {translated}");
                        finish?.Invoke(translated);
                    }
                }
                else
                {
                    Debug.LogError("❌ MyMemory API Error: " + www.error + " | Response: " + www.downloadHandler.text);
                }
            }
        }
        else
        {
            finish?.Invoke(sourceText);
            StopAllCoroutines();
        }
    }
}
