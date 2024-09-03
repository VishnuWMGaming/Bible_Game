using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using BibleGame;

public class MCQManager : MonoBehaviour,IOptionPanel,ICorrectPanel
{
    [Header("UI Settings:")]
    [SerializeField] OptionPanel optionPanel;
    [SerializeField] GameObject questionPanel;
    [SerializeField] CorrectPanel correctPanel;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Button _homeBtn;
    [SerializeField] Button _submitBtn;

    /// <summary>
    /// Action implemented one enable
    /// </summary>
    private void OnEnable()
    {
        optionPanel.callback = this;
        correctPanel.callback = this;

        _homeBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.home));
        _submitBtn.onClick.AddListener(() => optionPanel.CheckAnswerAction());

        _submitBtn.interactable = false;

        RestartAction();    
    }

    public void EnableSubmit(bool enabled)
    {
        _submitBtn.interactable = enabled;
    }

    /// <summary>
    /// Action imeplemented om disable
    /// </summary>
    private void OnDisable()
    {
        _homeBtn.onClick.RemoveAllListeners();
        _submitBtn.onClick.RemoveAllListeners();
    }

    public void CorrectAnswerAction()
    {
        questionPanel.gameObject.SetActive(false);
        correctPanel.gameObject.SetActive(true);
    }

    public void RestartAction()
    {
        questionPanel.gameObject.SetActive(true);
        correctPanel.gameObject.SetActive(false);

        _submitBtn.interactable = false;
    }
}
