using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelPop : MonoBehaviour
{
    [SerializeField] Button closeButton;
    public Button CloseBtn => closeButton;

    [SerializeField] TMP_Text messageText;
    public TMP_Text messageTxt => messageText;

}
