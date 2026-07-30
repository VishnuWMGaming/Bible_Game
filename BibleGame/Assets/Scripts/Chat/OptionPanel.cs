
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using DG.Tweening;

public class OptionChatPanel : MonoBehaviour
{
    [Header("Options:")]
    [SerializeField] List<OptionChat> options = new List<OptionChat>();

    public void Set(List<IOptData> optDatas, IOptionChat callback)
    {
        optDatas
             .Select((data, index) => new { data, option = options[index] })
             .ToList()
             .ForEach(x => {
                              x.option.gameObject.SetActive(true);
                              x.option.Set(x.data.value, x.data.index, callback); 
                           });
    }


    private void OnDisable()
    {
        foreach (var opt in options) 
            opt.gameObject.SetActive(false);
    }
}

[Serializable]
public class IOptData
{
    public string value;
    public int index;
}
