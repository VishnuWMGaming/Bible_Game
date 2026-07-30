using System.Collections.Generic;
using System.Text;

/// <summary>
/// Tiny hand-rolled JSON parser for exactly one shape:
///   { "keyA": { "str": "str", ... }, "keyB": { "str": "str", ... } }
/// Avoids pulling in a full JSON library just to load the emoji map.
/// </summary>
public static class MiniJson
{
    public static Dictionary<string, Dictionary<string, string>> ParseNestedStringDict(string json)
    {
        int i = 0;
        var result = new Dictionary<string, Dictionary<string, string>>();
        SkipWhitespace(json, ref i);
        Expect(json, ref i, '{');
        SkipWhitespace(json, ref i);
        if (Peek(json, i) == '}') { i++; return result; }

        while (true)
        {
            SkipWhitespace(json, ref i);
            string key = ParseString(json, ref i);
            SkipWhitespace(json, ref i);
            Expect(json, ref i, ':');
            SkipWhitespace(json, ref i);
            var inner = ParseStringDict(json, ref i);
            result[key] = inner;
            SkipWhitespace(json, ref i);
            char c = Peek(json, i);
            if (c == ',') { i++; continue; }
            if (c == '}') { i++; break; }
        }
        return result;
    }

    private static Dictionary<string, string> ParseStringDict(string json, ref int i)
    {
        var dict = new Dictionary<string, string>();
        Expect(json, ref i, '{');
        SkipWhitespace(json, ref i);
        if (Peek(json, i) == '}') { i++; return dict; }

        while (true)
        {
            SkipWhitespace(json, ref i);
            string key = ParseString(json, ref i);
            SkipWhitespace(json, ref i);
            Expect(json, ref i, ':');
            SkipWhitespace(json, ref i);
            string value = ParseString(json, ref i);
            dict[key] = value;
            SkipWhitespace(json, ref i);
            char c = Peek(json, i);
            if (c == ',') { i++; continue; }
            if (c == '}') { i++; break; }
        }
        return dict;
    }

    private static string ParseString(string json, ref int i)
    {
        Expect(json, ref i, '"');
        var sb = new StringBuilder();
        while (true)
        {
            char c = json[i++];
            if (c == '"') break;
            if (c == '\\')
            {
                char esc = json[i++];
                switch (esc)
                {
                    case '"': sb.Append('"'); break;
                    case '\\': sb.Append('\\'); break;
                    case '/': sb.Append('/'); break;
                    case 'b': sb.Append('\b'); break;
                    case 'f': sb.Append('\f'); break;
                    case 'n': sb.Append('\n'); break;
                    case 'r': sb.Append('\r'); break;
                    case 't': sb.Append('\t'); break;
                    case 'u':
                        string hex = json.Substring(i, 4);
                        i += 4;
                        int code = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                        sb.Append((char)code);
                        break;
                    default: sb.Append(esc); break;
                }
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    private static void SkipWhitespace(string json, ref int i)
    {
        while (i < json.Length && char.IsWhiteSpace(json[i])) i++;
    }

    private static char Peek(string json, int i) => i < json.Length ? json[i] : '\0';

    private static void Expect(string json, ref int i, char c)
    {
        if (json[i] != c)
            throw new System.Exception($"MiniJson: expected '{c}' at position {i}, got '{json[i]}'");
        i++;
    }
}
