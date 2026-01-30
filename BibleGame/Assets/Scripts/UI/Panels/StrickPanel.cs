using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using DebugUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BibleGame.API.GetBiblesAPI;

public class StrickPanel : MonoBehaviour,IStrick
{
    [SerializeField] Button newGame;

    [SerializeField] Button homeBtn;

    [SerializeField] TMP_Text userName;

    [Space]
    [SerializeField] PostImage iPic;


    [Header("Streak Settings:")]
    [SerializeField] GameObject strickObj;
    [SerializeField] Transform mStrkParent;
    [SerializeField] ScrollRect mStrkScroll;
    [SerializeField] GameObject noStreakObj;

    CancellationTokenSource cts;

    private void OnEnable()
    {
        ClearAll();

        mStrkScroll.verticalNormalizedPosition = 1;

        newGame?.onClick.AddListener(NewGame);
        homeBtn?.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.home); });
        

        userName.text = AppData.loginData.Name;

        if(AppData.loginData.Pic != null)
        iPic.SetRightSize(AppData.loginData.Pic, true);

        noStreakObj.SetActive(false);
        Init();
    }

    private void OnDisable()
    {
        newGame?.onClick.RemoveAllListeners();
        homeBtn?.onClick.RemoveAllListeners();

        ClearAll();
    }


    void NewGame()
    {
        Actions.ChangePanelActions(CanvasType.ageSelect);
    }


    async void Init()
    {
        cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        if (StreakAPI.streakDatas == null || StreakAPI.streakDatas.Count == 0)
        {
            noStreakObj.SetActive(true);
            return;
        }

        for (int i = 0; i < StreakAPI.streakDatas.Count; i++)
        {
            token.ThrowIfCancellationRequested();

            var data = StreakAPI.streakDatas[i];

            GameObject go = Instantiate(strickObj, mStrkParent);
            Strick strck = go.GetComponent<Strick>();

            var tcs = new TaskCompletionSource<(bool success, GetBiblesAPI.GetBibleDetailResponse res)>();

            GetBiblesAPI.GetDetail((success, res) =>
            {
                if (!token.IsCancellationRequested)
                    tcs.TrySetResult((success, res));

            }, data.bible_id);

            // Cancel-safe wait
            var result = await tcs.Task.WithCancellation(token);

            if (!result.success)
            {
                Debug.LogError("Error in getting bible details");
                return;
            }

            string bibleCode = result.res.ResponseData.data.nameLocal switch
            {
                "King James Version" => "KJV",
                "The Holy Bible, American Standard Version" => "ASV",
                _ => "NIL"
            };

            string age = data.age switch
            {
                "1" => "kindergarden",
                "2" => "Elementary",
                "3" => "Teenagers",
                _ => "Adult"
            };

            string title =
                $"<b>{bibleCode},{data.bible_id}</b> {data.percentage:F2}%\n" +
                $"<size=15>{age}\n{data.testament} Testamant</size>";

            strck.Init(title, i, this);
        }
    }

    public  void GetStreak(int index)
    {
        if (StreakAPI.streakDatas == null || StreakAPI.streakDatas.Count == 0)
            return;

        AudioManager.Instance.PlayButton();
        DevDebug.Log($"Streak index: {index}", DebugColor.Turquoise);

        StreakData streakData = StreakAPI.streakDatas[index];

        UserData.gameid = streakData._id;
        UserData.bookId = streakData.book_id;
        UserData.bibleId = streakData.bible_id;

        PopUp.Instance.EnableLoad(true);
        GetBiblesAPI.GetDetail((success, res) =>
        {
            PopUp.Instance.EnableLoad(false);

            if (!success)
            {
                Debug.LogError("Error in getting bible details");
            }

            DevDebug.Log($"Bible Name: {res.ResponseData.data.nameLocal}", DebugColor.Green);

            UserData.bibleName = res.ResponseData.data.nameLocal;

            UserData.currentAge = streakData.age switch
            {
                "1" => AgeGroup.kindergarden,
                "2" => AgeGroup.elementary,
                "3" => AgeGroup.teenagers,
                "4" => AgeGroup.adult,
                _ => AgeGroup.elementary
            };

            UserData.testament = streakData.testament switch
            {
                "Old" => Testament.Old,
                "New" => Testament.New,
                _ => throw new System.NotImplementedException(),
            };

            Debug.Log($"<color=cyan> testamant: {UserData.testament} </color>");

            UserData.coins = streakData.coins;

            Actions.ChangePanelActions(CanvasType.level);

        }, UserData.bibleId);
       
    }

    void ClearAll()
    {
        if (cts != null && !cts.IsCancellationRequested)
        {
            cts.Cancel();
            cts.Dispose();
        }

        if (mStrkParent.childCount <= 0)
            return;

        for(int i = 0; i< mStrkParent.childCount; i++)
        {
            GameObject go = mStrkParent.GetChild(i).gameObject;
            Destroy(go);
        }
    }
}