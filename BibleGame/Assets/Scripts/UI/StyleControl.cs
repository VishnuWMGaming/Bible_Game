using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using BibleGame.Data;

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
    [SerializeField] List<Image> mScores = new List<Image>();

    [Space]
    [SerializeField] List<Image> mcqs = new List<Image>();

    [Space]
    [SerializeField] List<Image> mcqs_Vertical = new List<Image>();

    [Space]
    [SerializeField] List<Image> panelsColor = new List<Image>();

    [Space]
    [SerializeField] List<Image> textButtons = new List<Image>();

    [Space]
    [SerializeField] List<Image> hintButtons = new List<Image>();

    [Space]
    [SerializeField] Image key;

    [Space]
    [SerializeField] List<Image> panels = new List<Image>();


    [Header("SpriteData")]
    [SerializeField] SpriteData spriteData;

    StyleUI styleUI;

    private void OnEnable()
    {
        styleUI = spriteData.GetStyle(UserData.currentAge);
        ApplyStyle();
    }

    void ApplyStyle()
    {
        if (styleUI == null)
            return;
        

        //Adding the styles
        foreach (var button in buttons) button.sprite = styleUI.button;
        foreach(var image in profiles) image.sprite = styleUI.profileSprite;

        if (bookTab != null) bookTab.sprite = styleUI.bookTab;

        if (mPage != null) mPage.sprite = styleUI.pageSprite;

        foreach(var score in mScores) score.sprite = styleUI.scoreSprite;

        foreach(var title in mtitles) title.color = styleUI.textColor;

        foreach (var panel in mcqs) panel.sprite = styleUI.mcqTabPanel;

        foreach (var panel in mcqs_Vertical) panel.sprite = styleUI.mcqTabPanel_Vertical;

        foreach (var panel in panelsColor)  panel.color = styleUI.textColor;

        foreach(var button in textButtons) button.sprite = styleUI.textButton;

        foreach (var button in hintButtons) button.sprite = styleUI.hintSprite;

        if(key != null) key.sprite = styleUI.keySprite;

        foreach (var panel in panels) panel.sprite = styleUI.panel;
    }
}
