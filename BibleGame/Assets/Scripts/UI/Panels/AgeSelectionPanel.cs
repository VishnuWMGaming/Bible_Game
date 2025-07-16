using BibleGame;
using BibleGame.API;
using BibleGame.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class AgeSelectionPanel : MonoBehaviour,IAge
{
    [Header("Ages:")]
    [SerializeField] List<Age> ages = new List<Age>();

    [Space]
    [SerializeField] Button backBtn;

    private void OnEnable()
    {
        foreach(var age in ages)
        {
            age.callback = this;
        }

        backBtn?.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.home));
    }

    private void OnDisable()
    {
        backBtn?.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Action implemented on selecting the age
    /// </summary>
    /// <param name="group"></param>
    public void AgeSelection(AgeGroup group)
    {
        PopUp.Instance.EnableLoad(true);

        GetBiblesAPI.Get((success, res) =>
        {
            PopUp.Instance.EnableLoad(false);

            if (!success)
            {
                Debug.Log("Erorr in get bible");
                return;
            }


            UserData.currentAge = group;
            Actions.ChangePanelActions(CanvasType.bible);
        });
    }
};
