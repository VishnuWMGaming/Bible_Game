using BibleGame;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanel : MonoBehaviour
{
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button settingsBtn;

    private void OnEnable()
    {
        homeBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.home); });
        settingsBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.ChangePanelActions(CanvasType.setPanel); });
    }

    private void OnDisable()
    {
        settingsBtn.onClick.RemoveAllListeners();
        homeBtn.onClick.RemoveAllListeners();
    }
}