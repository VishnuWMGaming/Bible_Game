using System.IO;
using UnityEngine;

/// <summary>
/// Converts raw audio bytes (WAV or MP3) into an AudioClip entirely in memory.
/// Auto-detects format by header, so it works regardless of what your TTS
/// backend actually sends back. No disk writes, no UnityWebRequest, no native
/// plugins - safe for WebGL, Standalone, and mobile alike.
/// </summary>
public static class AudioBytesUtility
{
    public static AudioClip ToAudioClip(byte[] bytes, string clipName = "voice")
    {
        if (bytes == null || bytes.Length < 4)
        {
            Debug.LogError("Audio data is null or too short");
            return null;
        }

        bool isWav = bytes[0] == 'R' && bytes[1] == 'I' && bytes[2] == 'F' && bytes[3] == 'F';

        return isWav ? WavUtility.ToAudioClip(bytes, clipName) : Mp3Utility.ToAudioClip(bytes, clipName);
    }
}

/// <summary>
/// Decodes MP3 bytes into an AudioClip using NLayer (pure C#, MIT licensed,
/// no native code - see the bundled NLayer/ folder and LICENSE.txt).
/// </summary>
public static class Mp3Utility
{
    public static AudioClip ToAudioClip(byte[] mp3Bytes, string clipName = "voice")
    {
        try
        {
            using (var stream = new MemoryStream(mp3Bytes))
            using (var mpegFile = new NLayer.MpegFile(stream))
            {
                int channels = mpegFile.Channels;
                int sampleRate = mpegFile.SampleRate;

                // Read in chunks since total sample count may be unknown for VBR streams.
                var allSamples = new System.Collections.Generic.List<float>(mp3Bytes.Length * 2);
                float[] buffer = new float[4096];
                int samplesRead;

                while ((samplesRead = mpegFile.ReadSamples(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < samplesRead; i++)
                    {
                        allSamples.Add(buffer[i]);
                    }
                }

                if (allSamples.Count == 0)
                {
                    Debug.LogError("MP3 decode produced no samples");
                    return null;
                }

                AudioClip clip = AudioClip.Create(clipName, allSamples.Count / channels, channels, sampleRate, false);
                clip.SetData(allSamples.ToArray(), 0);
                return clip;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"MP3 decode failed: {e.Message}");
            return null;
        }
    }
}
