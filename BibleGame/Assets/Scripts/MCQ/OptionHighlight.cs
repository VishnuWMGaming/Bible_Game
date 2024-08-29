using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class OptionHighlight : MonoBehaviour
{
    [SerializeField] Option.OptionType optionType;

    Image image;

    private void OnEnable()
    {
        image = GetComponent<Image>();

        ChangeSet(Option.OptionType.worng);
    }

    public void ChangeSet(Option.OptionType optionType)
    {
        image.color = optionType switch
        {
            Option.OptionType.correct => new Color32(39, 251, 111, 100),
            Option.OptionType.worng => new Color32(54, 188, 228, 100)
        };
    }
}
