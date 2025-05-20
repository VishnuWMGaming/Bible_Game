using BibleGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class AgeSelectionPanel : MonoBehaviour,IAge
{
    [Header("Ages:")]
    [SerializeField] List<Age> ages = new List<Age>();

    private void OnEnable()
    {
        foreach(var age in ages)
        {
            age.callback = this;
        }
    }

    /// <summary>
    /// Action implemented on selecting the age
    /// </summary>
    /// <param name="group"></param>
    public void AgeSelection(AgeGroup group)
    {
        GameData.currentAge = group;
        Actions.ChangePanelActions(CanvasType.testament);
    }
};
