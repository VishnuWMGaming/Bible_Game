using UnityEngine;
using TMPro;
using UnityEngine.UI;


public interface IOptionChat
{
    public void GiveAnswer(int key, string value);
}

[RequireComponent(typeof(Button))]
public class OptionChat : MonoBehaviour
{
    [SerializeField] TMP_Text mtext;
    [SerializeField] Button button;

    IOptionChat callback;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void Set(string optiontxt,int index, IOptionChat callbackIN)
    {
        callback = callbackIN;

        mtext.text = optiontxt;

        if (button == null)
            return;                                        

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => callback.GiveAnswer(index, optiontxt));
    }
}
