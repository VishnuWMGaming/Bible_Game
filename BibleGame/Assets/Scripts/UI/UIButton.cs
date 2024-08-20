using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

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

}
