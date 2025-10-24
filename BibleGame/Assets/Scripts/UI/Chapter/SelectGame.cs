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

    [Space]
    [SerializeField] PostImage iPic;

    public ISelectGame CallbackSelectGame;
    
    private void OnEnable()
    {
        homeBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); Actions.StartPageAction(StartPage.chapter); });
        triviaBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); CallbackSelectGame.PlayTriviaGame(); });
        wordBtn.onClick.AddListener(() => { AudioManager.Instance.PlayButton(); CallbackSelectGame.PlayWordGame(); });

        userName.text = AppData.loginData.Name;

        if (AppData.loginData.Pic != null)
            iPic.SetRightSize(AppData.loginData.Pic, true);
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