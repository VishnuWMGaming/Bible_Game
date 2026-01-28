using System;  
using System.Collections;  
using UnityEngine;  
using UnityEngine.Networking;  
using UnityEngine.UI; // Only needed if you are using UnityEngine.UI components  

public class ImageDownloader : MonoBehaviour  
{  
    // Callback delegate type  
    public delegate void OnImageDownloaded(Texture2D texture);  

    // Method to download image from URL  
    public void DownloadImage(string url, OnImageDownloaded callback)  
    {  
        StartCoroutine(DownloadImageCoroutine(url, callback));  
    }  

    // Coroutine for downloading the image  
    private IEnumerator DownloadImageCoroutine(string url, OnImageDownloaded callback)  
    {  
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))  
        {  
            // Send the web request and wait for a response  
            yield return webRequest.SendWebRequest();  

            // Check for errors  
            if (webRequest.result != UnityWebRequest.Result.Success)  
            {  
                Debug.LogError($"Error downloading image: {webRequest.error} :: {url}");  
                callback?.Invoke(null); // Invoke callback with null if there's an error  
            }  
            else  
            {  
                // Obtain the texture and invoke the callback  
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);  
                callback?.Invoke(texture); // Invoke the callback with the downloaded texture  
            }  
        }  
    }  
}
