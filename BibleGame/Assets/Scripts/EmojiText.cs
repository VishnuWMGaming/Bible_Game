using TMPro;
using UnityEngine;

/// <summary>
/// Attach to any GameObject that has a plain TextMeshPro - Text (UI) component
/// (NOT an input field). Whatever raw text is on it - typed in the Inspector,
/// or set later from code - gets emoji-converted to sprites automatically.
///
/// Requires:
///   - "Rich Text" enabled on the TMP_Text component (it's on by default)
///   - The Sprite Asset assigned on this TMP_Text component (or set as the
///     project-wide Default Sprite Asset in TMP Settings)
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class EmojiText : MonoBehaviour
{
    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();

        // Converts whatever text is already sitting in the Inspector field
        // (e.g. if you typed "Yes!" directly, like in your screenshot).
        ProcessCurrentText();
    }

    /// <summary>
    /// Re-processes whatever is currently in the text field. Call this if you
    /// ever set _text.text directly elsewhere and need it re-converted.
    /// </summary>
    public void ProcessCurrentText()
    {
        _text.text = EmojiTextProcessor.Instance.Process(_text.text);
    }

    /// <summary>
    /// Preferred way to set text from other scripts/game code - converts
    /// emoji automatically as part of assignment.
    ///   emojiText.SetText("Nice job! ");
    /// </summary>
    public void SetText(string raw)
    {
        _text.text = EmojiTextProcessor.Instance.Process(raw);
    }
}
