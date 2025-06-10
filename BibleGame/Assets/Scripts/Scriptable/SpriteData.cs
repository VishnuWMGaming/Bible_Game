using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteData", menuName = "BibleGameStore/SpriteData")]
public class SpriteData : ScriptableObject
{
    [Header("Styles:")]
    [SerializeField] List<StyleUI> styles = new List<StyleUI>();

    /// <summary>
    /// Get the style based on age group
    /// </summary>
    /// <param name="ageGroup"></param>
    /// <returns></returns>
    public StyleUI GetStyle(AgeGroup ageGroup)
    {
        return styles.Find(x => x.ageGroup == ageGroup);
    }
}


[Serializable]
public class StyleUI
{
    public Sprite BgSprite;
    public Sprite profileSprite;
    public Sprite keySprite;
    public Sprite scoreSprite;
    public Sprite hintSprite;
    public Sprite button;
    public Color32 textColor;
    public Sprite textButton;
    public Sprite pageSprite;
    public Sprite panelkey;
    public Sprite panel;
    public Sprite selectPanel;
    public Sprite unselectPanel;
    public Sprite bookTab;
    public Sprite mcqTabPanel;
    public Sprite box;

    [Space]
    public AgeGroup ageGroup;
}