using BibleGame;
using UnityEngine;
using UnityEngine.UI;

public class SelectGame : MonoBehaviour
{
    [SerializeField] private Button triviaBtn;
    [SerializeField] private Button wordBtn;
    [SerializeField] private Button homeBtn;

    public ISelectGame CallbackSelectGame;
    
    private void OnEnable()
    {
        homeBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.home));
        triviaBtn.onClick.AddListener(() => CallbackSelectGame.PlayTriviaGame());
        wordBtn.onClick.AddListener(() => CallbackSelectGame.PlayWordGame());
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