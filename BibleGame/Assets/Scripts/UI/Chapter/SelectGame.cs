using BibleGame;
using BibleGame.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectGame : MonoBehaviour
{
    [SerializeField] private Button triviaBtn;
    [SerializeField] private Button wordBtn;
    [SerializeField] private Button homeBtn;

    [SerializeField] TMP_Text userName;

    public ISelectGame CallbackSelectGame;
    
    private void OnEnable()
    {
        homeBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.home));
        triviaBtn.onClick.AddListener(() => CallbackSelectGame.PlayTriviaGame());
        wordBtn.onClick.AddListener(() => CallbackSelectGame.PlayWordGame());

        userName.text = AppData.loginData.Name;
    }

    private void OnDisable()
    {
        homeBtn.onClick.RemoveAllListeners();
        triviaBtn.onClick.RemoveAllListeners();
        wordBtn.onClick.RemoveAllListeners();
    }
}

public interface ISelectGame
{
    public void PlayTriviaGame();
    public void PlayWordGame();
}