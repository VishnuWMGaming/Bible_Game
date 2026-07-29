using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IChat : MonoBehaviour
{
    [SerializeField] IChattype chattype;
    public IChattype Chattype => chattype;

    [SerializeField] TMP_Text mText;
    [SerializeField] Image mProfile;


    public void Set(string msg , IChattype chattype)
    {
        mText.text = msg;

        if (chattype == IChattype.bot)
            return;

    
    }
    
}

public enum IChattype { bot,user }