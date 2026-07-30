using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Detects real emoji characters (including multi-codepoint ZWJ sequences,
/// skin-tone modifiers, keycaps, flags, etc.) inside a plain string and
/// rewrites them as TextMeshPro <sprite name="..."> tags, so pasting or
/// typing an emoji automatically displays the matching sprite.
///
/// Usage:
///   string processed = EmojiTextProcessor.Instance.Process(rawInput);
///   tmpText.text = processed;
/// </summary>
public class EmojiTextProcessor : MonoBehaviour
{
    public static EmojiTextProcessor Instance { get; private set; }

    [Tooltip("TextAsset for emoji_unicode_map.json (place it in a Resources folder or assign directly)")]
    public TextAsset mapJson;

    // emoji sequence (as a string) -> sprite name
    private Dictionary<string, string> _exactMap;
    private Dictionary<string, string> _fallbackMap; // FE0F-stripped fallback

    // sequence lengths present, sorted longest-first, so ZWJ / multi-codepoint
    // sequences are matched before their shorter sub-sequences (e.g. match the
    // 3-person family emoji before matching a lone person emoji inside it)
    private int[] _lengthsDescending;

    [Serializable]
    private class MapWrapper
    {
        public Dictionary<string, string> exact;
        public Dictionary<string, string> fallback_no_fe0f;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadMap();
    }

    private void LoadMap()
    {
        TextAsset json = mapJson;
        if (json == null)
            json = Resources.Load<TextAsset>("emoji_unicode_map");

        if (json == null)
        {
            Debug.LogError("EmojiTextProcessor: emoji_unicode_map.json not assigned or found in Resources.");
            _exactMap = new Dictionary<string, string>();
            _fallbackMap = new Dictionary<string, string>();
            _lengthsDescending = new int[0];
            return;
        }

        // Unity's JsonUtility can't parse Dictionary directly, so we use a
        // tiny manual parser via MiniJsonDict below (no external deps).
        var parsed = MiniJson.ParseNestedStringDict(json.text);
        _exactMap = parsed.TryGetValue("exact", out var e) ? e : new Dictionary<string, string>();
        _fallbackMap = parsed.TryGetValue("fallback_no_fe0f", out var f) ? f : new Dictionary<string, string>();

        var lengthSet = new HashSet<int>();
        foreach (var k in _exactMap.Keys) lengthSet.Add(new System.Globalization.StringInfo(k).LengthInTextElements == 0 ? k.Length : k.Length);
        // Use raw char-length (not grapheme count) since we match on UTF-16 char runs.
        lengthSet.Clear();
        foreach (var k in _exactMap.Keys) lengthSet.Add(k.Length);
        foreach (var k in _fallbackMap.Keys) lengthSet.Add(k.Length);
        _lengthsDescending = lengthSet.OrderByDescending(x => x).ToArray();

        Debug.Log($"EmojiTextProcessor: loaded {_exactMap.Count} emoji mappings.");
    }

    /// <summary>
    /// Scans input for emoji sequences and replaces them with
    /// <sprite name="..."> tags. Non-emoji text passes through unchanged.
    /// </summary>
    public string Process(string input)
    {
        if (string.IsNullOrEmpty(input) || _exactMap == null || _exactMap.Count == 0)
            return input;

        var sb = new StringBuilder(input.Length + 16);
        int i = 0;
        while (i < input.Length)
        {
            bool matched = false;

            foreach (int len in _lengthsDescending)
            {
                if (len <= 0 || i + len > input.Length) continue;
                string candidate = input.Substring(i, len);

                if (_exactMap.TryGetValue(candidate, out string spriteName))
                {
                    sb.Append("<sprite name=\"").Append(spriteName).Append("\">");
                    i += len;
                    matched = true;
                    break;
                }

                string stripped = candidate.Replace("\uFE0F", "");
                if (stripped.Length > 0 && _fallbackMap.TryGetValue(stripped, out string spriteName2))
                {
                    sb.Append("<sprite name=\"").Append(spriteName2).Append("\">");
                    i += len;
                    matched = true;
                    break;
                }
            }

            if (!matched)
            {
                // Handle surrogate pairs correctly (most emoji are outside the BMP)
                if (char.IsHighSurrogate(input[i]) && i + 1 < input.Length && char.IsLowSurrogate(input[i + 1]))
                {
                    sb.Append(input[i]).Append(input[i + 1]);
                    i += 2;
                }
                else
                {
                    sb.Append(input[i]);
                    i += 1;
                }
            }
        }

        return sb.ToString();
    }
}
