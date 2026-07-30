using BibleGame.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class IChat : MonoBehaviour
{
    [SerializeField] IChattype chattype;
    public IChattype Chattype => chattype;

    [SerializeField] TMP_Text mText;
    [SerializeField] PostImage mProfile;
    [SerializeField] EmojiText emojiText;

    public void Set(string msg , IChattype chattype)
    {
        mText.text = msg;
        emojiText.ProcessCurrentText();

        if (chattype == IChattype.bot)
            return;

        if (AppData.loginData.Pic != null)
            mProfile.SetRightSize(AppData.loginData.Pic, true);
    }
    
}

public enum IChattype { bot,user }