using BibleGame.Data;
using TMPro;
using UnityEngine;

public class Abbrevation : MonoBehaviour
{
    [SerializeField] TMP_Text mP_Text;

    private void OnEnable()
    {
        mP_Text.text = $"<b>{UserData.bibleName}</b> \n {UserData.chapterId}";        
    }
}
