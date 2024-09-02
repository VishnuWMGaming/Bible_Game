using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


public interface ICorrectPanel
{
    public void RestartAction();
}

public class CorrectPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] Button restartBtn;

    public ICorrectPanel callback;

    /// <summary>
    /// Action implemented on enable
    /// </summary>
    private void OnEnable()
    {
        restartBtn.onClick.AddListener(() => callback.RestartAction());
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        restartBtn.onClick.RemoveAllListeners();
    }
}
