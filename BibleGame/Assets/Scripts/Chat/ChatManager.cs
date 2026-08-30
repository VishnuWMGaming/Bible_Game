
using BibleGame.Utility;
using Cysharp.Threading.Tasks;
using DebugUtils;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

using BibleGame;
using TMPro;
using BibleGame.Data;

public class ChatManager : MonoBehaviour,IOptionChat
{
    [SerializeField] IChat botChat;
    [SerializeField] IChat userChat;
    [SerializeField] Button mGiveAnswerBtn;
    [SerializeField] TMP_Text mTitle;


    [SerializeField] Transform chatTransform;
    [SerializeField] ScrollRect scrollRect;


    [Header("OptionPanel:")]
    [SerializeField] OptionChatPanel optionPanel;

    TextAsset textAsset;

    [Header("Store:")]
    [SerializeField] ChatAssetData chatAssetData;

    BotChatData mCurrentBot;

    private void OnEnable()
    {
        textAsset = null;
        textAsset = chatAssetData.Get(AppData.mCurrentVidChapter);

        mTitle.text = AppData.mCurrentVidChapter;

        if (textAsset == null)
        {
            Debug.LogError("Text asset is null empty ");
            return;
        }

        string content = textAsset.text;
        Dialogue.Initialize(textAsset);

        BotChat();

        AudioManager.Instance.MuteBG(false);
        mGiveAnswerBtn.onClick.AddListener(()=> { SetOptions(); AudioManager.Instance.PlayButton(); });
    }

    private void OnDisable()
    {
        AudioManager.Instance.MuteBG(true);
        mGiveAnswerBtn.onClick.RemoveAllListeners();


        for(int i = 0; i<chatTransform.childCount;++i)
        {
            Destroy(chatTransform.GetChild(i).gameObject);
        }
    }

    void BotChat()
    {
        mCurrentBot = Dialogue.ReadBotByOption(Dialogue.answerKey);
        string botChat = Dialogue.NewLineAlignment(mCurrentBot.data);

        //DevDebug.Log($"Bot conversation:{Dialogue.Pointer}",DebugColor.Gold);

        BotAction(botChat);   

    }

    public async UniTask BotAction(string text)
    {
       GameObject botObj =  Instantiate(botChat.gameObject, chatTransform);
       IChat chat = botObj.GetComponent<IChat>();

       chat.Set("typing...", IChattype.bot);

       await UniTask.Delay(2000);
      
       chat.Set(text, IChattype.bot);

       RefreshLayout(chatTransform.GetComponent<RectTransform>());

       mGiveAnswerBtn.interactable = mCurrentBot.mOptions.Count >0;
       if(mCurrentBot.mOptions.Count <= 0)
       {
            BotChat();
       }
    }

    public void UserAction(string text)
    {
        mGiveAnswerBtn.interactable = false;

        GameObject userObj = Instantiate(userChat.gameObject, chatTransform);

        IChat chat = userObj.GetComponent<IChat>();
        chat.Set(text, IChattype.user);

        RefreshLayout(chatTransform.GetComponent<RectTransform>());

        BotChat();
    }

    public void GiveAnswer(int key, string value)
    {
        AudioManager.Instance.PlayButton();

        if (key <= 0)
            return;

        Dialogue.answerKey = key;

        optionPanel.gameObject.SetActive(false);

        UserAction(value);
    }

    public void SetOptions()
    {
        optionPanel.gameObject.SetActive(true);

        optionPanel.Set(mCurrentBot.mOptions, this);
    }

    public void RefreshLayout(RectTransform layoutRoot)
    {
        Canvas.ForceUpdateCanvases();

        float targetHeight = LayoutUtility.GetPreferredHeight(layoutRoot);

        scrollRect.DOVerticalNormalizedPos(0, 0.5f).OnComplete(() =>
        {
           // LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
        });   
    }
}

public class BotChatData
{
    public string data;
    public List<IOptData> mOptions = new();
}



