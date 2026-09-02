
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

    private async Task OnEnable()
    {
        textAsset = null;
        textAsset = chatAssetData.Get(AppData.mCurrentVidChapter);

        Language language = AppData.mVidLang switch
        {
            VidLang.english => Language.English,
            VidLang.swahili => Language.Swahili,
            VidLang.spanish => Language.Spanish,
            VidLang.creole => Language.Kreyol,
            VidLang.french => Language.French
        };

        mTitle.text = await LanguageController.Instance.TranslateAsync(AppData.mCurrentVidChapter,language);

        if (textAsset == null)
        {
            Debug.LogError("Text asset is null empty ");
            return;
        }

        string content = textAsset.text;
        Dialogue.Initialize(textAsset);

        BotChat();

        AudioManager.Instance.MuteBG(false);

        string mGiTxt = await LanguageController.Instance.TranslateAsync("Give Answer", language);

        mGiveAnswerBtn.GetComponentInChildren<TMP_Text>().text = mGiTxt;

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

        Language language = AppData.mVidLang switch
        {
            VidLang.english => Language.English,
            VidLang.swahili => Language.Swahili,
            VidLang.spanish => Language.Spanish,
            VidLang.creole => Language.Kreyol,
            VidLang.french => Language.French
        };

       string mbot = await LanguageController.Instance.TranslateAsync("typing...",language);
       chat.Set(mbot, IChattype.bot);

       await UniTask.Delay(2000);

       text = await LanguageController.Instance.TranslateAsync(text,language);
        chat.Set(text, IChattype.bot);

        foreach (IOptData data in mCurrentBot.mOptions)
            data.value = await LanguageController.Instance.TranslateAsync(data.value, language);


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

    public async Task SetOptions()
    {
        optionPanel.gameObject.SetActive(true);

        mGiveAnswerBtn.interactable = false;

        optionPanel.Set(mCurrentBot.mOptions, this);
        mGiveAnswerBtn.interactable = true;
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



