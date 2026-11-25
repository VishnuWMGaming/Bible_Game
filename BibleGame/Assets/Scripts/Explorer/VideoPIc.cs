using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public interface IVidPic
{
    public void InitVid(VideoPIc videoPIc);
}

[RequireComponent(typeof(Button))]
public class VideoPIc : MonoBehaviour
{
    string mUrl;
    public string URL => mUrl;

    [SerializeField] Image thumbnailImage;
    [SerializeField] TMP_Text titleText;

    [SerializeField] Button button;

    public event Action<Sprite, string> OnMetaFetched;

    public IVidPic callback;

    [Header("Video Objects:")]
    [SerializeField] List<VidObj> vidObjs = new List<VidObj>();

    [SerializeField] string mTitle;
    public string Title => mTitle;


    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button = GetComponent<Button>();
    }

    public void Init (string url, List<VidObj> objs,IVidPic callbackIN)
    {
        //mUrl = vidUrl;

        string videoId = ExtractVideoId(url);

        if (string.IsNullOrEmpty(videoId))
        {
            Debug.LogError("[YouTube] Could not parse video ID: " + url);
            return;
        }

        vidObjs = objs;

        callback = callbackIN;

        StartCoroutine(FetchAll(videoId, url));
    }

    public string GetVidUrl(VidLang lang)
    {
        VidObj obj = vidObjs.Find(x => x.lang == lang);

        return obj == null ? null :  vidObjs.Find(x => x.lang == lang).vidStoreLink;
    }

    public static string ExtractVideoId(string url)
    {
        if (string.IsNullOrEmpty(url)) return null;
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri)) return null;

        string host = uri.Host.ToLowerInvariant();

        // youtu.be/VIDEOID
        if (host.Contains("youtu.be"))
        {
            string id = uri.AbsolutePath.Trim('/');
            // Strip any trailing path segments (e.g. youtu.be/ID/extra)
            int slashIndex = id.IndexOf('/');
            return slashIndex >= 0 ? id.Substring(0, slashIndex) : id;
        }

        // youtube.com, m.youtube.com, music.youtube.com, www.youtube.com, etc.
        if (host.Contains("youtube.com"))
        {
            string[] segments = uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // /shorts/VIDEOID, /embed/VIDEOID, /live/VIDEOID
            if (segments.Length >= 2 &&
                (segments[0] == "shorts" || segments[0] == "embed" || segments[0] == "live"))
            {
                return segments[1];
            }

            // /watch?v=VIDEOID (also handles &list=, &t=, etc. after it)
            if (!string.IsNullOrEmpty(uri.Query))
            {
                foreach (string param in uri.Query.TrimStart('?').Split('&'))
                {
                    int eqIndex = param.IndexOf('=');
                    if (eqIndex < 0) continue;

                    string key = param.Substring(0, eqIndex);
                    string value = param.Substring(eqIndex + 1);

                    if (key == "v" && !string.IsNullOrEmpty(value))
                        return value;
                }
            }
        }

        return null;
    }

    IEnumerator FetchAll(string videoId, string originalUrl)
    {
        Sprite sprite = null;
        string title = "Unknown";

        titleText.text = "Loading...";
        button.interactable = false;

        Coroutine c1 = StartCoroutine(FetchSprite(videoId, s => sprite = s));
        Coroutine c2 = StartCoroutine(FetchTitle(originalUrl, t => title = t));

        yield return c1;
        yield return c2;

        button.interactable = true;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            if (callback == null) return;

            callback?.InitVid(this);
        });

        ApplySprite(thumbnailImage, sprite);

        //if (thumbnailImage != null && sprite != null)
        //    thumbnailImage.sprite = sprite;

        if (titleText != null)
            titleText.text = title;

        mTitle = title;

        OnMetaFetched?.Invoke(sprite, title);

        Debug.Log($"[YouTube] Title : {title}");
        Debug.Log($"[YouTube] Sprite: {(sprite != null ? $"{sprite.rect.width}x{sprite.rect.height}" : "null")}");
    }

    IEnumerator FetchSprite(string videoId, Action<Sprite> callback)
    {
        string[] qualities = { "maxresdefault", "hqdefault", "mqdefault" };

        foreach (string quality in qualities)
        {
            string url = $"https://img.youtube.com/vi/{videoId}/{quality}.jpg";

            using UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                continue;

            Texture2D tex = DownloadHandlerTexture.GetContent(req);

            // 120x90 = YouTube's grey placeholder for unavailable quality
            if (tex.width <= 120)
            {
                Destroy(tex);
                continue;
            }

            callback(TextureToSprite(tex));
            yield break;
        }

        Debug.LogWarning("[YouTube] All thumbnail qualities failed.");
        callback(null);
    }

    [System.Serializable]
    public class YouTubeOEmbedResponse
    {
        public string title;
        public string author_name;
        public string thumbnail_url;
    }

    IEnumerator FetchTitle(string pageUrl, Action<string> callback)
    {
        string oembedUrl = "https://www.youtube.com/oembed?url=" +
                            UnityWebRequest.EscapeURL(pageUrl) + "&format=json";

        using UnityWebRequest req = UnityWebRequest.Get(oembedUrl);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[YouTube] oEmbed fetch failed: " + req.error);
            callback("Unknown");
            yield break;
        }

        YouTubeOEmbedResponse data = JsonUtility.FromJson<YouTubeOEmbedResponse>(req.downloadHandler.text);
        string title = !string.IsNullOrEmpty(data?.title) ? data.title : "Unknown";

        callback(title);
    }

    static string ParseTitleFromHtml(string html)
    {
        // Method 1: og:title meta tag  →  most reliable
        Match og = Regex.Match(html,
            @"<meta\s+property=""og:title""\s+content=""([^""]+)""",
            RegexOptions.IgnoreCase);

        if (og.Success)
            return DecodeHtml(og.Groups[1].Value);

        // Method 2: <title> tag  →  appends " - YouTube" suffix
        Match t = Regex.Match(html,
            @"<title>([^<]+)</title>",
            RegexOptions.IgnoreCase);

        if (t.Success)
        {
            string raw = t.Groups[1].Value;
            // Strip the " - YouTube" suffix
            int suffix = raw.LastIndexOf(" - YouTube", StringComparison.OrdinalIgnoreCase);
            return DecodeHtml(suffix >= 0 ? raw.Substring(0, suffix) : raw);
        }

        // Method 3: JSON-LD / ytInitialData title field
        Match js = Regex.Match(html,
            @"""title""\s*:\s*""([^""\\]*(?:\\.[^""\\]*)*)""");

        if (js.Success)
            return DecodeHtml(js.Groups[1].Value);

        return "Unknown";
    }

    // ── Decode common HTML entities ──────────────────────────────
    static string DecodeHtml(string text)
    {
        return text
            .Replace("&amp;", "&")
            .Replace("&quot;", "\"")
            .Replace("&#39;", "'")
            .Replace("&lt;", "<")
            .Replace("&gt;", ">")
            .Replace("\\u0026", "&")
            .Replace("\\\"", "\"");
    }

    public static Sprite TextureToSprite(Texture2D tex)
    {
        // Re-encode to fix raw JPEG bottom-left origin
        Texture2D correct = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);

        Color[] src = tex.GetPixels();
        Color[] dst = new Color[src.Length];

        int w = tex.width;
        int h = tex.height;

        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                dst[y * w + x] = src[(h - 1 - y) * w + x];

        correct.SetPixels(dst);
        correct.Apply();
        Destroy(tex);

        return Sprite.Create(
            correct,
            new Rect(0, 0, correct.width, correct.height),
            new Vector2(0.5f, 0.5f),   // center pivot
            100f,                       // pixels per unit
            0,                          // extrude edges
            SpriteMeshType.FullRect     // no mesh trimming
        );
    }


    void ApplySprite(Image image, Sprite sprite)
    {
        if (image == null || sprite == null) return;

        image.sprite = sprite;
        image.type = Image.Type.Simple;
        image.preserveAspect = true;

        float aspect = sprite.rect.width / sprite.rect.height;

        // Add or get AspectRatioFitter
        AspectRatioFitter fitter = image.GetComponent<AspectRatioFitter>();
        if (fitter == null)
            fitter = image.gameObject.AddComponent<AspectRatioFitter>();

        fitter.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
        fitter.aspectRatio = aspect;
    }
}
