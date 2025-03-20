using System;
using System.Collections;
using System.Collections.Generic;
using BibleGame;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnagramManager : MonoBehaviour, ICorrectPanel, IAnagramManager
{
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button hintBtn;
    [SerializeField] private Button submitBtn;
    [SerializeField] private Button hintSubmitBtn;
    [SerializeField] private Button hintCloseBtn;
    [SerializeField] private Button rewardBtn;
    [SerializeField] GameObject hintPanel;


    private void OnEnable()
    {
        hintBtn.onClick.AddListener(OnClickHintBtn);
        hintPanel.SetActive(false);
        homeBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.home));
        rewardBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.reward));
        // submitBtn.onClick.AddListener(() => optionPanel.CheckAnswerAction());

        submitBtn.interactable = false;
        submitBtn.onClick.AddListener(NextAction);
        
        hintBtn.onClick.AddListener(ShowHint);
        hintSubmitBtn.onClick.AddListener(UseHint);
        hintCloseBtn.onClick.AddListener(CloseHint);

    }

    private void OnDisable()
    {
        homeBtn.onClick.RemoveAllListeners();
        hintBtn.onClick.RemoveAllListeners();
        submitBtn.onClick.RemoveListener(NextAction);
    }

    private void OnClickHomeBtn()
    {
        SceneManager.LoadScene(0);
    }

    private void OnClickHintBtn()
    {
        hintPanel.SetActive(true);
    }

    private void CloseHint()
    {
        hintPanel.SetActive(false);
    }
    private void ShowHint()
    {
        hintPanel.SetActive(true);
    }

    private void UseHint()
    {
        
    }


    public void RestartAction()
    {
        
    }

    public void NextAction()
    {
        Actions.ChangePanelActions(CanvasType.home);
    }

    public void ActivateSubmitBtn()
    {
        submitBtn.interactable = true;
    }

    public void DeactivateSubmitBtn()
    {
        submitBtn.interactable = false;
    }
}

public interface IAnagramManager
{
    public void ActivateSubmitBtn();
    public void DeactivateSubmitBtn();
}
