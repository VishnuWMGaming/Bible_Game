using BibleGame.Data;
using DebugUtils;
using TMPro;
using UnityEngine;

public class Abbrevation : MonoBehaviour
{
    [SerializeField] TMP_Text mP_Text;

    private void OnEnable()
    {
        DevDebug.Log($"Abbrevation: {UserData.bibleName} : {UserData.chapterId}",DebugColor.Silver);

        string bibleCode = UserData.bibleName switch
        {
            "King James Version" => "KJV",
            "The Holy Bible, American Standard Version" =>"ASV"
        };

        mP_Text.text = $"<b>{bibleCode}</b>,{UserData.chapterId}";        
    }
}
