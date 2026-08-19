using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class Video : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    private VideoPlayer videoPlayer;
    public RawImage renderImg;

    [SerializeField]
    private string url;

    public string URL => url;

    [Header("UI")]
    public Slider progressSlider;

    public TMP_Text currentTimeText;
    public TMP_Text durationText;
    [SerializeField] TMP_Text mtitle;

    public Button playPauseButton;
    public Button replayButton;
    public Button chatBtn;

    [SerializeField] private Sprite pauseSprite;
    [SerializeField] private Sprite playSprite;

    [Space]
    [SerializeField] GameObject mLoadingPanel;

    private CancellationTokenSource cancellationTokenSource;
    private bool isDragging = false;

    private void OnEnable()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.playOnAwake = false;
        videoPlayer.waitForFirstFrame = true;

        progressSlider.onValueChanged.RemoveListener(OnSliderChanged);
        progressSlider.onValueChanged.AddListener(OnSliderChanged);

        playPauseButton.onClick.AddListener(TogglePlayPause);

        mLoadingPanel.SetActive(true);

        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnDisable()
    {
        playPauseButton.onClick.RemoveAllListeners();
        progressSlider.onValueChanged.RemoveAllListeners();

        videoPlayer.loopPointReached -= OnVideoFinished;
        chatBtn.onClick.RemoveListener(ChatFunction);
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        replayButton.gameObject.SetActive(true);

        replayButton.onClick.RemoveAllListeners();
        replayButton.onClick.AddListener(() =>
        {
            replayButton.gameObject.SetActive(false);

            progressSlider.value = 0;

            currentTimeText.text = "00";
            durationText.text = "00";

            Color32 color = renderImg.color;
            renderImg.color = new Color32(color.r, color.g, color.b, 0);

            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();

            mLoadingPanel?.SetActive(true);
            StartVideo(url);
        });
    }

    public void SetTitle(string title)
    {
        mtitle.text = title;
        chatBtn.interactable = title == "Chester's Garage - Summer Camp Pt. 1 (S4E1)";

        chatBtn.onClick.AddListener(ChatFunction);
    }

    // START VIDEO
    public async void StartVideo(string vidurl)
    {
        url = vidurl;

        cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await LoadVideo(url, cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Video loading cancelled");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    // MAIN LOADER (ARCHIVE + GOOGLE DRIVE + DIRECT MP4)
    private async Task LoadVideo(string url, CancellationToken token)
    {
        videoPlayer.Stop();

        string streamUrl = null;
        bool isGoogleDrive = url.Contains("drive.google.com") || url.Contains("drive.usercontent.google.com");
        bool isArchive = url.Contains("archive.org");

        // ----------------------------
        // ✅ INTERNET ARCHIVE HANDLING
        // ----------------------------
        if (isArchive)
        {
            streamUrl = ConvertArchiveUrl(url);
        }
        // ----------------------------
        // ✅ GOOGLE DRIVE HANDLING (large-file bypass)
        // ----------------------------
        else if (isGoogleDrive)
        {
            string fileId = ExtractGoogleDriveFileId(url);

            if (string.IsNullOrEmpty(fileId))
            {
                Debug.LogError("Could not extract Google Drive file ID from: " + url);
                return;
            }

            try
            {
                streamUrl = await ResolveGoogleDriveDownloadUrlAsync(fileId, token);
            }
            catch (Exception ex)
            {
                Debug.LogError("Google Drive resolve failed: " + ex.Message);
                return;
            }
        }
        else
        {
            // fallback: direct video URL
            streamUrl = url;
        }

        if (string.IsNullOrEmpty(streamUrl))
        {
            Debug.LogError("No playable stream found");
            return;
        }

        Debug.Log("Streaming URL: " + streamUrl);

        Color32 color = renderImg.color;

        // ----------------------------
        // VIDEO PLAYER SETUP
        // ----------------------------
        if (isArchive)
        {
            // Archive URLs still benefit from redirect resolution + cleanup.
            string finalUrl = await ResolveFinalUrlAsync(streamUrl, token);
            finalUrl = CleanArchiveUrl(finalUrl);
            ApplyVideoSource(finalUrl);
        }
        else
        {
            // Google Drive stream URL is already the resolved, direct-download
            // URL with a confirm token — no further redirect resolution needed.
            ApplyVideoSource(streamUrl);
        }

        while (!videoPlayer.isPrepared)
        {
            token.ThrowIfCancellationRequested();
            await Task.Delay(100, token);
        }

        // UI SETUP
        progressSlider.minValue = 0;
        progressSlider.maxValue = (float)videoPlayer.length;

        durationText.text = FormatTime(videoPlayer.length);

        videoPlayer.Play();
        mLoadingPanel.SetActive(false);

        renderImg.color = new Color32(color.r, color.g, color.b, 255);

        UpdatePlayPauseText();
    }

    private void ApplyVideoSource(string finalUrl)
    {
        AudioManager.Instance.MuteBG(true);

        Debug.Log("Final resolved URL: " + finalUrl);

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = finalUrl;

        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetDirectAudioMute(0, false);
        videoPlayer.SetDirectAudioVolume(0, 1f);

        videoPlayer.Prepare();
    }

    // ----------------------------
    // ARCHIVE URL CONVERTER
    // ----------------------------
    private string ConvertArchiveUrl(string url)
    {
        try
        {
            Uri uri = new Uri(url);
            string fullPath = uri.AbsolutePath;

            string[] parts = fullPath.Split("/details/");
            if (parts.Length < 2)
                return url;

            string itemAndFile = parts[1];
            string[] split = itemAndFile.Split('/', 2);
            if (split.Length < 2)
                return url;

            string itemId = split[0];
            string fileName = split[1];

            fileName = Uri.UnescapeDataString(fileName);
            fileName = Uri.EscapeDataString(fileName);

            string downloadUrl = $"https://archive.org/download/{itemId}/{fileName}";

            return downloadUrl;
        }
        catch (Exception ex)
        {
            Debug.LogError("Archive URL conversion failed: " + ex.Message);
            return url;
        }
    }

    // ----------------------------
    // GOOGLE DRIVE - LARGE FILE BYPASS
    // ----------------------------
    // Drive serves an HTML "can't scan for viruses" interstitial for big
    // files instead of raw bytes. This fetches that page, extracts the
    // confirm token (and uuid, if present), and rebuilds the URL so it
    // returns the actual video stream instead of the warning page.
    //
    // ⚠️ Still fragile: Drive can change this markup, and files with
    // restricted sharing or extra verification steps won't resolve this way.
    // For production reliability, prefer the Drive REST API
    // (files.get?alt=media) with an API key/OAuth token, or host the file
    // on your own CDN/storage instead.
    private async Task<string> ResolveGoogleDriveDownloadUrlAsync(string fileId, CancellationToken token)
    {
        string initialUrl = $"https://drive.usercontent.google.com/download?id={fileId}&export=download";

        using (UnityWebRequest request = UnityWebRequest.Get(initialUrl))
        {
            request.redirectLimit = 10;

            await SendRequestAsync(request, token);

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception("Initial Drive request failed: " + request.error);

            string contentType = request.GetResponseHeader("Content-Type");

            // If Drive already returned the actual file (small file, no
            // warning page), Content-Type won't be text/html — we're done.
            if (contentType != null && !contentType.Contains("text/html"))
            {
                return request.url;
            }

            string html = request.downloadHandler.text;

            string confirmToken = null;

            Match confirmMatch = Regex.Match(html, @"confirm=([0-9A-Za-z_-]+)");
            if (confirmMatch.Success)
                confirmToken = confirmMatch.Groups[1].Value;

            if (string.IsNullOrEmpty(confirmToken))
            {
                Match formConfirm = Regex.Match(html, @"name=""confirm""\s+value=""([^""]+)""");
                if (formConfirm.Success)
                    confirmToken = formConfirm.Groups[1].Value;
            }

            string uuid = null;
            Match uuidMatch = Regex.Match(html, @"(?:name=""uuid""\s+value=""|uuid=)([0-9A-Za-z_-]+)");
            if (uuidMatch.Success)
                uuid = uuidMatch.Groups[1].Value;

            if (string.IsNullOrEmpty(confirmToken))
            {
                throw new Exception("Could not find confirm token — Drive page format may have changed, or file requires manual permission/sign-in.");
            }

            string finalUrl = $"https://drive.usercontent.google.com/download?id={fileId}&export=download&confirm={confirmToken}";

            if (!string.IsNullOrEmpty(uuid))
                finalUrl += $"&uuid={uuid}";

            return finalUrl;
        }
    }

    private string ExtractGoogleDriveFileId(string url)
    {
        // /file/d/{id}/...
        Match match = Regex.Match(url, @"/file/d/([a-zA-Z0-9_-]+)");
        if (match.Success)
            return match.Groups[1].Value;

        // ?id={id}  (covers /open?id= and /uc?id=)
        match = Regex.Match(url, @"[?&]id=([a-zA-Z0-9_-]+)");
        if (match.Success)
            return match.Groups[1].Value;

        return null;
    }

    // ----------------------------
    // GENERIC UnityWebRequest -> Task BRIDGE
    // ----------------------------
    private Task SendRequestAsync(UnityWebRequest request, CancellationToken token)
    {
        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine(SendRequestCoroutine(request, tcs, token));
        return tcs.Task;
    }

    private IEnumerator SendRequestCoroutine(UnityWebRequest request, TaskCompletionSource<bool> tcs, CancellationToken token)
    {
        var op = request.SendWebRequest();

        while (!op.isDone)
        {
            if (token.IsCancellationRequested)
            {
                request.Abort();
                tcs.TrySetCanceled(token);
                yield break;
            }
            yield return null;
        }

        tcs.TrySetResult(true);
    }

    // ----------------------------
    // ARCHIVE REDIRECT RESOLUTION
    // ----------------------------
    private async Task<string> ResolveFinalUrlAsync(string url, CancellationToken token)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.redirectLimit = 10;

            await SendRequestAsync(request, token);

            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.url; // resolved redirected URL
            }
            else
            {
                Debug.LogWarning("URL resolve failed, using original: " + request.error);
                return url;
            }
        }
    }

    void ChatFunction()
    {
        videoPlayer.Pause();
        UpdatePlayPauseText();
    }

    // PLAY / PAUSE
    public void TogglePlayPause()
    {
        if (videoPlayer.isPlaying)
            videoPlayer.Pause();
        else
            videoPlayer.Play();

        UpdatePlayPauseText();
    }

    private void UpdatePlayPauseText()
    {
        playPauseButton.image.sprite =
            videoPlayer.isPlaying ? pauseSprite : playSprite;
    }

    // SEEK
    public void Forward10Seconds()
    {
        if (!videoPlayer.isPrepared)
            return;

        videoPlayer.time += 10;
    }

    public void Backward10Seconds()
    {
        if (!videoPlayer.isPrepared)
            return;

        videoPlayer.time = Math.Max(0, videoPlayer.time - 10);
    }

    // SLIDER
    public void OnSliderChanged(float value)
    {
        videoPlayer.time = value;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        videoPlayer.time = progressSlider.value;
    }

    private void Update()
    {
        if (videoPlayer.isPrepared)
        {
            progressSlider.SetValueWithoutNotify((float)videoPlayer.time);
            currentTimeText.text = FormatTime(videoPlayer.time);
        }
    }

    // TIME FORMAT
    private string FormatTime(double time)
    {
        int minutes = Mathf.FloorToInt((float)time / 60);
        int seconds = Mathf.FloorToInt((float)time % 60);

        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    private string CleanArchiveUrl(string url)
    {
        return Uri.UnescapeDataString(url)
            .Replace("+", "%20")
            .Replace("%27", "")
            .Replace("(", "%28")
            .Replace(")", "%29");
    }

    public void CloseAction()
    {
        videoPlayer.Stop();
        gameObject.SetActive(false);

        progressSlider.value = 0;

        currentTimeText.text = "00";
        durationText.text = "00";

        AudioManager.Instance.MuteBG(false);

        Color32 color = renderImg.color;
        renderImg.color = new Color32(color.r, color.g, color.b, 0);

        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }

    private void OnDestroy()
    {
        videoPlayer.Stop();
        gameObject.SetActive(false);

        progressSlider.value = 0;

        currentTimeText.text = "00";
        durationText.text = "00";

        Color32 color = renderImg.color;
        renderImg.color = new Color32(color.r, color.g, color.b, 0);

        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }
}