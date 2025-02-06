using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Options Color Template", fileName = "OptionColors")]
public class OptionColors : ScriptableObject
{
    [Header("Normal")] 
    public Color normText;
    public Color normBorder;
    public Color normBg;

    [Header("Selected")] 
    public Color selectedText;
    public Color selectedBorder;
    public Color selectedBg;
    
    [Header("Hint")]
    public Color hintText;
    public Color hintBorder;
    public Color hintBg;
}
