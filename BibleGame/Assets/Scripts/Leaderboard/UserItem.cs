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

public interface IUserItem
{
    public void ShowInfo(string id);
}


public class UserItem : MonoBehaviour
{
    public PostImage profileImg;
    public TMP_Text nameTxt;
    public TMP_Text rankTxt;

    public GameObject loadingObj;

    public ImageDownloader imageDownloader;

    [Space]
    [SerializeField] string mID;

    [SerializeField] Button mbutton;

    private IUserItem callback;

    public async void Initialize(string id, string name, int rank, string spriteurl, IUserItem callbackIN)
    {
        DevDebug.Log($"Id: {id} :: {spriteurl}", DebugColor.Gold);
        mID = id;

        callback = callbackIN;

        mbutton?.onClick.RemoveAllListeners();
        mbutton?.onClick.AddListener(() =>
        {
            DevDebug.Log($"Id: called {id}", DebugColor.Gold);
            callback.ShowInfo(id);
        });

        nameTxt.text = name;
 
        rankTxt.text = rank >= 0? rank.ToString() : "NULL";

        profileImg.Reset();

        loadingObj.SetActive(true);
        Sprite sprite = spriteurl == null ? null : await DownloadSpriteAsync($"{spriteurl}");
        loadingObj.SetActive(false);

        if (sprite != null)
        {
            profileImg.SetRightSize(sprite, true);
        }

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
