using System;
using System.Collections;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        replayButton.gameObject.SetActive(true);

        replayButton.onClick.RemoveAllListeners();
        replayButton.onClick.AddListener(() =>
        {
            replayButton.gameObject.SetActive(false);
            //CloseAction();

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

    public void SetTitle(string title) { mtitle.text = title; }

    // START VIDEO
    public async void StartVideo(string vidurl)
    {
        url = vidurl;

        //cancellationTokenSource?.Cancel();
        //cancellationTokenSource?.Dispose();

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

    // MAIN LOADER (ARCHIVE + DIRECT MP4)
    private async Task LoadVideo(string url, CancellationToken token)
    {
        videoPlayer.Stop();

        string streamUrl = null;

        // ----------------------------
        // ✅ INTERNET ARCHIVE HANDLING
        // ----------------------------
        if (url.Contains("archive.org"))
        {
            streamUrl = ConvertArchiveUrl(url);
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
        StartCoroutine(ResolveFinalUrl(streamUrl, (finalUrl) =>
        {
            
            finalUrl = CleanArchiveUrl(finalUrl);

            AudioManager.Instance.MuteBG(true);

            Debug.Log("Final resolved URL: " + finalUrl);

            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = finalUrl;

            videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.SetDirectAudioMute(0, false);
            videoPlayer.SetDirectAudioVolume(0, 1f);

            videoPlayer.Prepare();
        }));

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

    // ----------------------------
    // ARCHIVE URL CONVERTER
    // ----------------------------
    private string ConvertArchiveUrl(string url)
    {
        try
        {
            // Example input:
            // https://archive.org/details/.../file.mp4

            Uri uri = new Uri(url);

            string fullPath = uri.AbsolutePath;

            // Extract after /details/
            string[] parts = fullPath.Split("/details/");

            if (parts.Length < 2)
                return url;

            string itemAndFile = parts[1];

            // split item name + file name
            string[] split = itemAndFile.Split('/', 2);

            if (split.Length < 2)
                return url;

            string itemId = split[0];
            string fileName = split[1];

            // Decode + re-encode properly
            fileName = Uri.UnescapeDataString(fileName);
            fileName = Uri.EscapeDataString(fileName);

            string downloadUrl =
                $"https://archive.org/download/{itemId}/{fileName}";

            return downloadUrl;
        }
        catch (Exception ex)
        {
            Debug.LogError("Archive URL conversion failed: " + ex.Message);
            return url;
        }
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
       // if (isDragging)
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

    private IEnumerator ResolveFinalUrl(string url, Action<string> onDone)
    {
        using (UnityEngine.Networking.UnityWebRequest request =
               UnityEngine.Networking.UnityWebRequest.Get(url))
        {
            request.redirectLimit = 10;

            yield return request.SendWebRequest();

            string finalUrl = url;

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                finalUrl = request.url; // resolved redirected URL
            }
            else
            {
                Debug.LogWarning("URL resolve failed, using original: " + request.error);
            }

            onDone?.Invoke(finalUrl);
        }
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