using BibleGame;
using BibleGame.Data;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BibleExplorerPanel : MonoBehaviour,IVidPic
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] Button backButton;

    [SerializeField] VideoPIc videoPIc;
    [SerializeField] Transform videoPicTransform;

    [Space]
    [SerializeField] Video mVideoPlayer;

    [Header("VideoStore")]
    [SerializeField] VideoUrlData videoUrlData;

    [Header("Vidlang")]
    [SerializeField] GameObject mVideoLangPanel;

    VideoPIc mVideoPic;

    private void OnEnable()
    {
        backButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayButton();

            ClearAll();
            Actions.ChangePanelActions(CanvasType.home);
        });

        Init();
        Actions.SetVidLang += VideoInit;
    }

    private void OnDisable()
    {
        ClearAll();
        backButton.onClick.RemoveAllListeners();

        Actions.SetVidLang -= VideoInit;
    }

    void Init()
    {
        List<VidData> list = videoUrlData.GetAll();

        for (int i = 0; i < list.Count; i++)
        {
            string vidyoutubeUrl = list[i].youtubeLink;
            //string vidStorUrl = list[i].vidStoreLink;

            GameObject go = Instantiate(videoPIc.gameObject,videoPicTransform);
            go.transform.localScale = Vector3.one;

            VideoPIc video = go.GetComponent<VideoPIc>();

            video.Init(vidyoutubeUrl, list[i].objs,this);
        }
    }

    public void InitVid(VideoPIc videoPIc)
    {
        AudioManager.Instance.PlayButton();

        mVideoLangPanel.SetActive(true); 
        mVideoPic = videoPIc;

        //mVideoPlayer.gameObject.SetActive(true);
        //mVideoPlayer.StartVideo(vidUrl);

        //mVideoPlayer.SetTitle(title);
    }

    private void VideoInit(VidLang lang)
    {
        if (mVideoPic == null)
            return;

        string vidUrl = mVideoPic.GetVidUrl(lang);

        if(string.IsNullOrWhiteSpace(vidUrl))
        {
            PopUp.Instance.ShowMessage("No video available !!");
            return;
        }

        AppData.mVidLang = lang;

        mVideoLangPanel.SetActive(false);

        mVideoPlayer.gameObject.SetActive(true);
        mVideoPlayer.StartVideo(vidUrl);

        mVideoPlayer.SetTitle(mVideoPic.Title);
    }

    void ClearAll()
    {
        if (videoPicTransform.childCount <= 0)
            return;

        for (int i = 0; i < videoPicTransform.childCount; i++)
        {
            GameObject go = videoPicTransform.GetChild(i).gameObject;

            Destroy(go);
        }
    }
}
