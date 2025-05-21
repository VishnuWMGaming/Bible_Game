using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class StyleControl : MonoBehaviour
{
    [Header("UI Settings:")]
    [SerializeField] List<Image> buttons  = new List<Image>();

    [Space]
    [SerializeField] Image background;

    [Space]
    [SerializeField] Image mPage;

    [Space]
    [SerializeField] List<Image> profiles = new List<Image>();

    [Space]
    [SerializeField] Image bookTab;

    [Space]
    [SerializeField] List<TMP_Text>  mtitles = new List<TMP_Text>();

    [Space]
    [SerializeField] Image mScore;
    [SerializeField] Image mcq;

    [Space]
    [SerializeField] List<Image> panels = new List<Image>();

    [Space]
    [SerializeField] List<Image> textButtons = new List<Image>();

    [Space]
    [SerializeField] Image hintButton;


    [Header("SpriteData")]
    [SerializeField] SpriteData spriteData;

    StyleUI styleUI;

    private void OnEnable()
    {
        styleUI = spriteData.GetStyle(GameData.currentAge);
        ApplyStyle();
    }

    void ApplyStyle()
    {
        if (styleUI == null)
            return;
        
        foreach (var button in buttons) button.sprite = styleUI.button;
        foreach(var image in profiles) image.sprite = styleUI.profileSprite;

        if (bookTab != null) bookTab.sprite = styleUI.bookTab;

        if (mPage != null) mPage.sprite = styleUI.pageSprite;

        if(mScore != null) mScore.sprite = styleUI.scoreSprite;

        foreach(var title in mtitles) title.color = styleUI.textColor;

        if (mcq != null) mcq.sprite = styleUI.mcqTabPanel;

        foreach(var panel in panels)  panel.color = styleUI.textColor;

        foreach(var button in textButtons) button.sprite = styleUI.textButton;

        if (hintButton != null) hintButton.sprite = styleUI.hintSprite;
    }
}
