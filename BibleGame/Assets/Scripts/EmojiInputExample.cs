using TMPro;
using UnityEngine;

/// <summary>
/// Example: user types/pastes into an input field, and a separate
/// TMP_Text display shows the same text with emoji auto-swapped for sprites.
/// Attach to any GameObject and wire up the two references in the Inspector.
/// </summary>
public class EmojiInputExample : MonoBehaviour
{
    public TMP_InputField input;   // where the user types raw text/emoji
    public TMP_Text display;       // where the processed text (with sprites) is shown

    private void OnEnable()
    {
        if (input != null)
            input.onValueChanged.AddListener(OnTextChanged);
    }

    private void OnDisable()
    {
        if (input != null)
            input.onValueChanged.RemoveListener(OnTextChanged);
    }

    private void OnTextChanged(string raw)
    {
        if (display == null) return;
        display.text = EmojiTextProcessor.Instance.Process(raw);
    }

    // If you just want to process a string once (e.g. a chat message
    // arriving from a server) rather than live-as-you-type:
    public static string ProcessOnce(string raw)
    {
        return EmojiTextProcessor.Instance.Process(raw);
    }
}
