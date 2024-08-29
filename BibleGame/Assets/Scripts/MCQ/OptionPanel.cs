using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionPanel : MonoBehaviour,IOption
{
    [Header("Options:")]
    [SerializeField] List<Option> options = new List<Option>();

    private void OnEnable()
    {
        for (int i = 0; i < options.Count; i++) 
            options[i].callback = this;
    }

    /// <summary>
    /// To lock all the options
    /// </summary>
    public void OptionLockAction()
    {
        for (int i = 0; i < options.Count; i++)
            options[i].EnableButtonInteraction(false);
    }
}
