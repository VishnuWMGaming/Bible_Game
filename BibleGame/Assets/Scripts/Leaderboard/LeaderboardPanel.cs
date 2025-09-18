using UnityEngine;
using UnityEngine.UI;
using BibleGame;
using PolyAndCode.UI;
using System;
using System.Collections.Generic;

public class LeaderboardPanel : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [Header("UI Settings:")]
    [SerializeField] Button backButton;
    public RecyclableScrollRect recyclableScrollRect;
    public List<LeaderboardData> leaderboardDataList = new List<LeaderboardData>();
    void Start()
    {
#if TESTING
        for (int i = 0; i < 100; i++)
        {
            if (i == 30)
            {
                leaderboardDataList.Add(new LeaderboardData($"Vivek", i + 1));
            }
            else
            {
                leaderboardDataList.Add(new LeaderboardData($"Player{i + 1}", i + 1));
            }
        }
#endif
        Invoke("Initialize", 0.3f);
    }
    private void Initialize()
    {
        recyclableScrollRect.Initialize(this);
    }
    private void OnEnable()
    {
        // Back button → Go to Home
        backButton?.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayButton();
            Actions.ChangePanelActions(CanvasType.home);
        });
    }

    private void OnDisable()
    {
        backButton?.onClick.RemoveAllListeners();
    }

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
        leaderboardItem.Initialize(name, rank);
    }

    public void PageChanged(int index)
    {

    }
}
[Serializable]
public class LeaderboardData
{
    public string name;
    public int rank;
    public LeaderboardData(string name, int rank)
    {
        this.name = name;
        this.rank = rank;
    }
}