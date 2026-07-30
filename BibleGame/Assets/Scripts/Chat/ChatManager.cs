
using BibleGame.Utility;
using Cysharp.Threading.Tasks;
using DebugUtils;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour,IOptionChat
{
    [SerializeField] IChat botChat;
    [SerializeField] IChat userChat;
    [SerializeField] Button mGiveAnswerBtn;

    [SerializeField] Transform chatTransform;
    [SerializeField] ScrollRect scrollRect;


    [Header("OptionPanel:")]
    [SerializeField] OptionChatPanel optionPanel;

    [Header("TextAsset:")]
    [SerializeField] TextAsset textAsset;

    BotChatData mCurrentBot;

    private void OnEnable()
    {
        if (textAsset == null)
        {
            Debug.LogError("Text asset is null empty ");
            return;
        }

        string content = textAsset.text;
        Dialogue.Initialize(textAsset);

        BotChat();

        mGiveAnswerBtn.onClick.AddListener(SetOptions);
    }

    private void OnDisable()
    {
        mGiveAnswerBtn.onClick.RemoveAllListeners();
    }

    void BotChat()
    {
        mCurrentBot = Dialogue.ReadNextBot();
        string botChat = Dialogue.NewLineAlignment(mCurrentBot.data);

        DevDebug.Log($"Bot conversation:{Dialogue.Pointer}",DebugColor.Gold);

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

       mGiveAnswerBtn.interactable = true;  
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
        if (key <= 0)
            return;

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

        scrollRect.DOVerticalNormalizedPos(0, 0.5f);

        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
    }
}

public class BotChatData
{
    public string data;
    public List<IOptData> mOptions = new();
}



