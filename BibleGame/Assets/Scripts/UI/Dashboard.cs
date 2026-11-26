using System;
using System.Collections.Generic;
using UnityEngine;

public class Dashboard : MonoBehaviour, IUIButton
{

    [Header("Buttons")]
    [SerializeField] List<UIButton> buttons = new List<UIButton>();

    private void OnEnable()
    {
        foreach (var button in buttons)
        {
            button.callback = this;
        }
    }

    private void OnDisable()
    {
        
    }

    public void SelectAction(UIButton iButton)
    {
        foreach (var button in buttons)
        {
            if(button != iButton)
            {
                button.UnSelectAction();
            }
        }
    }
}
