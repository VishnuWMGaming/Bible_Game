using BibleGame;
using DebugUtils;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItem : MonoBehaviour, ICell
{
    public PostImage profileImg;
    public TMP_Text nameTxt;
    public TMP_Text rankTxt;

    public GameObject loadingObj;

    public ImageDownloader imageDownloader;

    public async Task Initialize(string name, int rank, string spriteurl)
    {
        nameTxt.text = name;
        rankTxt.text = rank.ToString();

        profileImg.Reset();

        loadingObj.SetActive(true);
        Sprite sprite = spriteurl == "0" ? null : await DownloadSpriteAsync($"{spriteurl}");
        loadingObj.SetActive(false);

        if (sprite != null)
        {
            profileImg.SetRightSize(sprite, true);
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public async Task<Sprite> DownloadSpriteAsync(string url)
    {
        var tcs = new TaskCompletionSource<Sprite>();

        Debug.Log($"<color=cyan>Image URL: {url}</color>");

        imageDownloader.DownloadImage(url, (tex) =>
        {
            if (tex != null)
            {
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                tcs.SetResult(sprite);
            }
            else
            {
                tcs.SetResult(null);
            }
        });

        return await tcs.Task;
    }
}
