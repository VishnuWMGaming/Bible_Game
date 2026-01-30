using BibleGame;
using BibleGame.API;
using DebugUtils;
using PolyAndCode.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static BibleGame.API.LeaderBoardAPI;

public class LeaderboardPanel : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [Header("UI Settings:")]
    [SerializeField] Button backButton;
    [SerializeField] RecyclableScrollRect recyclableScrollRect;
    [SerializeField] List<LeaderboardData> leaderboardDataList = new List<LeaderboardData>();

    [Header("ImageLoader:")]
    [SerializeField] ImageDownloader imageDownloader;

    [Header("Leaders")]
    [SerializeField] TopLeader firstLeader;
    [SerializeField] TopLeader secondLeader;
    [SerializeField] TopLeader thirdLeader;

    [SerializeField] UserItem mUserItem;


    private SynchronizationContext unitySyncContext;


    private void Awake()
    {
        unitySyncContext = SynchronizationContext.Current;
    }

    private void OnEnable()
    {
        // Back button → Go to Home
        backButton?.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayButton();
            Actions.ChangePanelActions(CanvasType.home);
        });


        PopUp.Instance.EnableLoad(true);
        LeaderBoardAPI.GetData(async (success, res) =>
        {
       
            if (!success)
            {
                PopUp.Instance.EnableLoad(false);
                PopUp.Instance.ShowMessage("Error in fetching in leaderboard");
                return;
            }

            if (res.ResponseData.Count <= 0)
            {
                PopUp.Instance.EnableLoad(false);
                PopUp.Instance.ShowMessage("No users found in the ranks");
                return;
            }

            Sprite firstsprite = res.ResponseData[0].profile_pic == "0" ? null : await DownloadSpriteAsync($"{ServiceURL.imageURL}{res.ResponseData[0].profile_pic}");
            firstLeader.Init(res.ResponseData[0].user_name, res.ResponseData[0].score, firstsprite);

            Sprite secondsprite = res.ResponseData[1].profile_pic == "0" ? null : await DownloadSpriteAsync($"{ServiceURL.imageURL}{res.ResponseData[1].profile_pic}");
            secondLeader.Init(res.ResponseData[1].user_name, res.ResponseData[1].score, secondsprite);

            Sprite thirdsprite = res.ResponseData[2].profile_pic == "0" ? null : await DownloadSpriteAsync($"{ServiceURL.imageURL}{res.ResponseData[2].profile_pic}");
            thirdLeader.Init(res.ResponseData[2].user_name, res.ResponseData[2].score, thirdsprite);

            for (int i = 3; i < res.ResponseData.Count; i++)
            {
                //Sprite sprite = res.ResponseData[i].profile_pic == "0" ? null : await DownloadSpriteAsync($"{ServiceURL.imageURL}{res.ResponseData[i].profile_pic}");'

                string mSpriteUrl = res.ResponseData[i].profile_pic == "0" ? "0" : $"{ServiceURL.imageURL}{res.ResponseData[i].profile_pic}";

                LeaderboardData data = new LeaderboardData(res.ResponseData[i].user_name, res.ResponseData[i].rank, mSpriteUrl);
                leaderboardDataList.Add(data);
            }

            RankUser user = res.ResponseData.Find(x => x.user_name.ToLower().Contains(GetProfileAPI.profilData.name.ToLower()));

            if (res != null)
            {
                string mSpriteUrl = user.profile_pic == "0" ? "0" : $"{ServiceURL.imageURL}{user.profile_pic}";

                DevDebug.Log("User is present in the leaderboard !!!", DebugColor.Coral);
                mUserItem.Initialize(user.user_name, user.rank, mSpriteUrl);
            }

            PopUp.Instance.EnableLoad(false);
            Invoke("Initialize", 0.3f);
        });
    }

    private void OnDisable()
    {
        backButton?.onClick.RemoveAllListeners();
    }



    private void Start()
    {
     
    }

    private void Initialize()
    {
        recyclableScrollRect.Initialize(this);
    }

    #region SCROLL
    public int GetStartingIndex()
    {
        return 30;
    }

    public int GetItemCount()
    {
        return leaderboardDataList.Count;
    }

    public void SetCell(ICell cell, int index)
    {
        LeaderboardItem leaderboardItem = cell as LeaderboardItem;
        string name = leaderboardDataList[index].name;
        int rank = leaderboardDataList[index].rank;

        leaderboardItem.Initialize(name, rank, leaderboardDataList[index].pic);
    }
    
    public void PageChanged(int index)
    {

    }

    #endregion

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
[Serializable]
public class LeaderboardData
{
    public string name;
    public int rank;
    public string pic;

    public LeaderboardData(string name, int rank,string pic)
    {
        this.name = name;
        this.rank = rank;
        this.pic = pic;
    }
}