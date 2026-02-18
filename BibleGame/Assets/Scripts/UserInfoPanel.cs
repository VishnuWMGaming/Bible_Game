using BibleGame;
using BibleGame.API;
using BibleGame.Utility;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UserInfoPanel : MonoBehaviour
{
    [SerializeField] TMP_Text uName;
    [SerializeField] TMP_Text uGameStreaks;
    [SerializeField] TMP_Text uCoinsEarned;
    [SerializeField] TMP_Text uLevelsFinished;

    [Space]
    [SerializeField] PostImage mPostImg;
    [SerializeField] GameObject mImgLoad;

    [Space]
    [SerializeField] ImageDownloader imageDownloader;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        Clean();
    }

    void Clean()
    {
        uName.text = "";
        uGameStreaks.text = "";
        uCoinsEarned.text = "";
        uLevelsFinished.text = "";

        mPostImg.Reset();
    }

    public void Init(string id)
    {
        mPostImg.Reset();

        if (string.IsNullOrWhiteSpace(id))
            return;

        PopUp.Instance.EnableLoad(true);
        UserInfoAPI.Get(async (success,res) =>
        {
            PopUp.Instance.EnableLoad(false);
            if (!success)
                return;

           mImgLoad.SetActive(true);
           uName.text = $"<color=blue>Name: </color><color=black>{res.ResponseData.user_name}</color>";
           uGameStreaks.text = $"<color=blue>GameStreaks: </color><color=black>{res.ResponseData.total_game_streak}</color>";
           uCoinsEarned.text = $"<color=blue>Coins: </color><color=black>{res.ResponseData.total_coins_earned}</color>";
           uLevelsFinished.text = $"<color=blue>Levels: </color><color=black>{res.ResponseData.levels_finished}</color>";

            Sprite sprite = res.ResponseData.profile_pic == "0" ? null : await DownloadSpriteAsync($"{ServiceURL.imageURL}{res.ResponseData.profile_pic}");

            mImgLoad.SetActive(false);
            if (sprite == null)
                return;

            mPostImg.SetRightSize(sprite, true);

        }, id);
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
