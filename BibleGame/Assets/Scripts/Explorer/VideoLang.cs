using UnityEngine;
using UnityEngine.UI;

using BibleGame;

[RequireComponent(typeof(Button))]
public class VideoLang : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] VidLang lang;

    private void OnEnable()
    {
        button?.onClick.RemoveAllListeners();
        button?.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayButton();
            Actions.SetVidLang?.Invoke(lang);
        });
    }

}
