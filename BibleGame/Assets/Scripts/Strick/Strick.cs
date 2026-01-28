using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IStrick
{
    public void GetStreak(int index);
}

public class Strick : MonoBehaviour
{
    [SerializeField] TMP_Text detail;
    [SerializeField] int index;

    [SerializeField] Button button;
    public Button MButton => button;

    IStrick callback;

    private void OnEnable()
    {
        detail.text = "Loading..."; 
    }

    public void Init(string val,int ind,IStrick callbackIN)
    {
        detail.text = val;
        index = ind;

        callback = callbackIN;

        button.onClick.AddListener(() =>
        {
            if (callback == null)
                return;

            callback.GetStreak(index);
        });
    }
}
