using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using DG.Tweening;
using UnityEngine.EventSystems;


[RequireComponent(typeof(Button))]
public class UIButton : MonoBehaviour
{
    [SerializeField] TMP_Text buttonText;
    public TMP_Text ButtonText =>  buttonText;

    Button button;
    public Button GetButton => button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    #region EVENT_FUNCTIONS
   
    #endregion
}
