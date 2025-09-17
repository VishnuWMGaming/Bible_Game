using UnityEngine;
using UnityEngine.UI;
using BibleGame;

public class LeaderboardPanel : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] Button backButton;

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
}
