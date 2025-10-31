using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using DG.Tweening;

[RequireComponent(typeof(Image))]
public class OptionHighlight : MonoBehaviour
{
    [SerializeField] Option.OptionType optionType;

    Image image;

    Color32 defaultcolor;

    private void Awake()
    {
        image = GetComponent<Image>();
        defaultcolor = image.color;
    }

    private void OnEnable()
    {

       // ChangeSet(Option.OptionType.worng);
    }

    public void ChangeSet(Option.OptionType optionType)
    {
        Color value = optionType switch
        {
            Option.OptionType.correct => new Color32(39, 251, 111, 100),
            Option.OptionType.wrong => Color.red,
            Option.OptionType.disable => new Color32(167, 167, 167, 100),
            _ => defaultcolor
        };

        image.color = value;
        this.optionType = optionType;

        Color32 color = image.color;

        color.a = 100;
        image.color = color;
    }
}
