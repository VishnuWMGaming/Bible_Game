using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Local HTTP relay for Google Drive public share-link videos.
///
/// IMPORTANT LESSON LEARNED: doing the "resolve confirm token" step with one
/// HTTP client (e.g. UnityWebRequest) and the "actual download" step with a
/// DIFFERENT HTTP client (e.g. HttpClient) is unreliable — Drive's decision
/// to serve the warning page vs. the real file can depend on session/cookie
/// state that does NOT carry over between two independent clients, even for
/// the exact same URL.
///
/// This class therefore does BOTH steps — token resolution AND download —
/// using ONE single HttpClient/cookie session, and makes at most two total
/// requests to Drive: (1) a lightweight check/warning-page fetch, and, only
/// if needed, (2) the real download with the extracted confirm token. MF /
/// VideoPlayer never talks to Drive directly at all — only to this local
/// server, which serves bytes from a locally cached file.
/// </summary>
public class DriveStreamProxy
{
    private TcpListener _listener;
    private CancellationTokenSource _cts;
    private readonly HttpClient _http;

    private string _fileId;
    private string _localPath;
    private string _contentType = "video/mp4";
    private long _contentLength = -1;

    private long _bytesDownloaded = 0;
    private Exception _downloadError = null;
    private bool _downloadComplete = false;

    private const int SocketTimeoutMs = 15000;
    private const int WaitForBytesPollMs = 100;

    public int Port { get; private set; }

    private readonly HttpClientHandler _handler;

    public DriveStreamProxy()
    {
        _handler = new HttpClientHandler { AllowAutoRedirect = true, UseCookies = true, CookieContainer = new CookieContainer() };
        _http = new HttpClient(_handler);
        _http.Timeout = Timeout.InfiniteTimeSpan; // long-lived download, not a quick request

        // A bare HttpClient sends almost no headers by default — no User-Agent,
        // no Accept, no Accept-Language, no Referer. That looks nothing like a
        // real browser, and Drive's abuse detection can respond to large public
        // files more defensively for headerless/non-browser-looking requests,
        // independent of actual per-file download counts. Sending ordinary
        // browser-equivalent headers is standard HTTP client configuration.
        _http.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
        _http.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,video/webm,video/mp4,*/*;q=0.8");
        _http.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
        _http.DefaultRequestHeaders.Referrer = new Uri("https://drive.google.com/");
    }

    /// <summary>
    /// Starts the relay for a given Google Drive file ID. Performs token
    /// resolution + starts the download using ONE shared HttpClient/session,
    /// waits only until headers of the real download are validated, then
    /// returns the local URL for VideoPlayer while the body streams to disk
    /// in the background.
    /// </summary>
    public async Task<string> StartAsync(string fileId, CancellationToken outerToken)
    {
        _fileId = fileId;
        _cts = CancellationTokenSource.CreateLinkedTokenSource(outerToken);

        string fileName = "drive_cache_" + Guid.NewGuid().ToString("N") + ".mp4";
        _localPath = Path.Combine(Application.temporaryCachePath, fileName);

        Debug.Log($"[DriveStreamProxy] Resolving + downloading fileId={fileId} to {_localPath} (single session)...");

        var headersReadyTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        _ = Task.Run(() => ResolveAndDownloadAsync(headersReadyTcs, _cts.Token), _cts.Token);

        using (outerToken.Register(() => headersReadyTcs.TrySetCanceled()))
        {
            await headersReadyTcs.Task; // throws if resolution/validation failed
        }

        _listener = new TcpListener(IPAddress.Loopback, 0);
        _listener.Start();
        Port = ((IPEndPoint)_listener.LocalEndpoint).Port;

        Debug.Log($"[DriveStreamProxy] Listening on 127.0.0.1:{Port}, contentLength={_contentLength}, contentType={_contentType}");

        _ = AcceptLoopAsync(_cts.Token);

        return $"http://127.0.0.1:{Port}/video.mp4";
    }

    /// <summary>
    /// Does the full Drive handshake on ONE HttpClient/session:
    /// 1. GET the plain download URL.
    /// 2. If it's already real video (small file, no warning page) — use it.
    /// 3. If it's HTML (large-file warning) — extract the confirm token from
    ///    THIS SAME response, then make the real request with that token,
    ///    still on the same client/cookie jar.
    /// Then streams the real response body to the local cache file.
    /// </summary>
    private async Task ResolveAndDownloadAsync(TaskCompletionSource<bool> headersReadyTcs, CancellationToken token)
    {
        try
        {
            string plainUrl = $"https://drive.usercontent.google.com/download?id={_fileId}&export=download";

            HttpResponseMessage resp = await _http.GetAsync(plainUrl, HttpCompletionOption.ResponseHeadersRead, token);
            string ct = resp.Content.Headers.ContentType?.MediaType;

            Debug.Log($"[DriveStreamProxy] Step 1 response: {resp.StatusCode}, Content-Type={ct}, ContentLength={resp.Content.Headers.ContentLength}");

            if (ct != null && ct.Contains("html", StringComparison.OrdinalIgnoreCase))
            {
                // Large-file warning page. First check for a download_warning_*
                // cookie — this is the primary mechanism Google uses and is what
                // proven tools like gdown check first. Only fall back to parsing
                // the HTML body if no such cookie was set.
                string confirmToken = null;
                var cookies = _handler.CookieContainer.GetCookies(new Uri("https://drive.usercontent.google.com"));
                foreach (Cookie c in cookies)
                {
                    if (c.Name.StartsWith("download_warning", StringComparison.OrdinalIgnoreCase))
                    {
                        confirmToken = c.Value;
                        Debug.Log($"[DriveStreamProxy] Found confirm token in cookie '{c.Name}'.");
                        break;
                    }
                }

                string html = await resp.Content.ReadAsStringAsync();
                resp.Dispose();

                if (string.IsNullOrEmpty(confirmToken))
                    confirmToken = ExtractConfirmToken(html);

                if (string.IsNullOrEmpty(confirmToken))
                {
                    bool looksLikeQuota = html.Contains("Quota exceeded", StringComparison.OrdinalIgnoreCase);
                    bool looksLikeSignIn = html.Contains("Sign in", StringComparison.OrdinalIgnoreCase) && html.Contains("accounts.google.com");

                    string reason = looksLikeQuota
                        ? "Google Drive's anonymous download quota for THIS SPECIFIC FILE is temporarily exhausted (too many anonymous downloads of this file recently — often from repeated testing). This is per-file and usually resets within ~24 hours, or is fixed by making a fresh copy of the file in Drive."
                        : looksLikeSignIn
                            ? "This file appears to require sign-in / isn't properly public. Check its sharing setting is 'Anyone with the link'."
                            : "Drive's warning page format may have changed, or this file has an unusual restriction.";

                    Debug.LogError($"[DriveStreamProxy] Could not find confirm token. Diagnosis: {reason}\nFirst 500 chars of body:\n" +
                                    html.Substring(0, Math.Min(500, html.Length)));
                    throw new Exception($"Could not extract Drive confirm token. {reason}");
                }

                Debug.Log($"[DriveStreamProxy] Extracted confirm token: {confirmToken}. Making real download request on same session...");

                string realUrl = $"https://drive.usercontent.google.com/download?id={_fileId}&export=download&confirm={confirmToken}";
                resp = await _http.GetAsync(realUrl, HttpCompletionOption.ResponseHeadersRead, token);
                ct = resp.Content.Headers.ContentType?.MediaType;

                Debug.Log($"[DriveStreamProxy] Step 2 (real) response: {resp.StatusCode}, Content-Type={ct}, ContentLength={resp.Content.Headers.ContentLength}");

                if (ct != null && ct.Contains("html", StringComparison.OrdinalIgnoreCase))
                {
                    string html2 = await resp.Content.ReadAsStringAsync();
                    Debug.LogError("[DriveStreamProxy] Still got HTML after confirm token. First 500 chars:\n" +
                                    html2.Substring(0, Math.Min(500, html2.Length)));
                    throw new Exception("Drive still returned HTML even after using the confirm token. " +
                                         "The file may require sign-in, be restricted, or Drive's flow changed.");
                }
            }

            // At this point `resp` holds the real video response (either the
            // Step 1 response directly, for small files with no warning page,
            // or the Step 2 response after using the confirm token).
            using (resp)
            {
                long? len = resp.Content.Headers.ContentLength;
                if (!resp.IsSuccessStatusCode || len == null || len <= 0)
                {
                    throw new Exception($"Unexpected final Drive response: {resp.StatusCode}, ContentLength={len}");
                }

                _contentType = ct ?? "video/mp4";
                _contentLength = len.Value;

                headersReadyTcs.TrySetResult(true);

                using var httpStream = await resp.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(_localPath, FileMode.Create, FileAccess.Write, FileShare.Read, 81920, useAsync: true);

                byte[] buffer = new byte[81920];
                int read;
                long total = 0;
                while ((read = await httpStream.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, read, token);
                    total += read;
                    Interlocked.Exchange(ref _bytesDownloaded, total);
                }

                await fileStream.FlushAsync(token);

                if (total < _contentLength)
                {
                    _downloadError = new Exception($"Download ended early: got {total}/{_contentLength} bytes.");
                    Debug.LogError("[DriveStreamProxy] " + _downloadError.Message);
                    return;
                }

                _downloadComplete = true;
                Debug.Log($"[DriveStreamProxy] Download complete: {total} bytes written to {_localPath}");
            }
        }
        catch (Exception ex)
        {
            _downloadError = ex;
            headersReadyTcs.TrySetException(ex);
            Debug.LogError($"[DriveStreamProxy] Resolve/download failed: {ex}");
        }
    }

    private static string ExtractConfirmToken(string html)
    {
        // Try the most common current markup: an anchor/link with confirm=TOKEN.
        var match = Regex.Match(html, @"confirm=([0-9A-Za-z_-]+)");
        if (match.Success)
            return match.Groups[1].Value;

        // Fallback: hidden form field <input name="confirm" value="TOKEN">
        match = Regex.Match(html, @"name=""confirm""\s+value=""([^""]+)""");
        if (match.Success)
            return match.Groups[1].Value;

        return null;
    }

    public void Stop()
    {
        Debug.Log("[DriveStreamProxy] Stopping proxy.");
        try { _cts?.Cancel(); } catch { }
        try { _listener?.Stop(); } catch { }
        try
        {
            if (!string.IsNullOrEmpty(_localPath) && File.Exists(_localPath))
                File.Delete(_localPath);
        }
        catch { }
    }

    private async Task AcceptLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            TcpClient client;
            try { client = await _listener.AcceptTcpClientAsync(); }
            catch { break; }

            Debug.Log("[DriveStreamProxy] Accepted a client connection.");
            _ = Task.Run(() => HandleClientAsync(client, token), token);
        }
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken token)
    {
        using (client)
        using (var netStream = client.GetStream())
        {
            try
            {
                string requestLine = await ReadLineWithTimeoutAsync(netStream, token);
                Debug.Log($"[DriveStreamProxy] Request line: '{requestLine}'");
                if (string.IsNullOrEmpty(requestLine)) return;

                string rangeHeader = null;
                string line;
                while (!string.IsNullOrEmpty(line = await ReadLineWithTimeoutAsync(netStream, token)))
                {
                    if (line.StartsWith("Range:", StringComparison.OrdinalIgnoreCase))
                        rangeHeader = line.Substring("Range:".Length).Trim();
                }

                long rangeStart = 0;
                long rangeEnd = _contentLength - 1;

                if (!string.IsNullOrEmpty(rangeHeader) && rangeHeader.StartsWith("bytes=", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = rangeHeader.Substring(6).Split('-');
                    if (long.TryParse(parts[0], out var s)) rangeStart = s;
                    if (parts.Length > 1 && long.TryParse(parts[1], out var e)) rangeEnd = e;
                    if (rangeEnd >= _contentLength) rangeEnd = _contentLength - 1;
                    if (rangeStart > rangeEnd) rangeStart = rangeEnd;
                }

                bool isPartial = !string.IsNullOrEmpty(rangeHeader);
                long servedLength = rangeEnd - rangeStart + 1;

                Debug.Log($"[DriveStreamProxy] Serving bytes {rangeStart}-{rangeEnd} from local cache (downloaded so far: {Interlocked.Read(ref _bytesDownloaded)})");

                var headerBuilder = new StringBuilder();
                headerBuilder.Append(isPartial ? "HTTP/1.1 206 Partial Content\r\n" : "HTTP/1.1 200 OK\r\n");
                headerBuilder.Append($"Content-Type: {_contentType}\r\n");
                headerBuilder.Append("Accept-Ranges: bytes\r\n");
                headerBuilder.Append($"Content-Length: {servedLength}\r\n");
                if (isPartial)
                    headerBuilder.Append($"Content-Range: bytes {rangeStart}-{rangeEnd}/{_contentLength}\r\n");
                headerBuilder.Append("Connection: close\r\n\r\n");

                byte[] headerBytes = Encoding.ASCII.GetBytes(headerBuilder.ToString());
                await netStream.WriteAsync(headerBytes, 0, headerBytes.Length, token);

                using var fileStream = new FileStream(_localPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 81920, useAsync: true);
                fileStream.Seek(rangeStart, SeekOrigin.Begin);

                byte[] buffer = new byte[81920];
                long remaining = servedLength;
                long position = rangeStart;

                while (remaining > 0 && !token.IsCancellationRequested)
                {
                    if (_downloadError != null)
                        throw new Exception("Background download failed: " + _downloadError.Message);

                    long available = Interlocked.Read(ref _bytesDownloaded) - position;
                    if (available <= 0)
                    {
                        if (_downloadComplete) break;
                        await Task.Delay(WaitForBytesPollMs, token);
                        continue;
                    }

                    int toRead = (int)Math.Min(buffer.Length, Math.Min(available, remaining));
                    int read = await fileStream.ReadAsync(buffer, 0, toRead, token);
                    if (read <= 0)
                    {
                        await Task.Delay(WaitForBytesPollMs, token);
                        continue;
                    }

                    await netStream.WriteAsync(buffer, 0, read, token);
                    remaining -= read;
                    position += read;
                }

                Debug.Log($"[DriveStreamProxy] Finished serving range {rangeStart}-{rangeEnd}.");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[DriveStreamProxy] Client handler exception (often normal on seek/disconnect): {ex.Message}");
            }
        }
    }

    private static async Task<string> ReadLineWithTimeoutAsync(NetworkStream stream, CancellationToken token)
    {
        var readTask = ReadLineAsync(stream);
        var timeoutTask = Task.Delay(SocketTimeoutMs, token);
        var completed = await Task.WhenAny(readTask, timeoutTask);
        if (completed == timeoutTask)
            throw new TimeoutException("Timed out reading from client socket.");
        return await readTask;
    }

    private static async Task<string> ReadLineAsync(NetworkStream stream)
    {
        var sb = new StringBuilder();
        int prev = -1;
        var buf = new byte[1];
        while (await stream.ReadAsync(buf, 0, 1) != 0)
        {
            if (prev == '\r' && buf[0] == '\n') { sb.Length--; break; }
            sb.Append((char)buf[0]);
            prev = buf[0];
        }
        return sb.ToString();
    }
}