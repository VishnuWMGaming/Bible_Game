using System;
using UnityEngine;

/// <summary>
/// Converts raw WAV file bytes directly into an AudioClip, entirely in memory.
/// No disk writes, no UnityWebRequest, no native plugins - works identically
/// on WebGL, Standalone, and mobile.
/// </summary>
public static class WavUtility
{
    public static AudioClip ToAudioClip(byte[] wavBytes, string clipName = "voice")
    {
        if (wavBytes == null || wavBytes.Length < 44)
        {
            Debug.LogError("WAV data too short or null");
            return null;
        }

        int channels = BitConverter.ToInt16(wavBytes, 22);
        int sampleRate = BitConverter.ToInt32(wavBytes, 24);
        int bitsPerSample = BitConverter.ToInt16(wavBytes, 34);

        int dataChunkOffset = FindChunk(wavBytes, "data", out int dataChunkSize);
        if (dataChunkOffset < 0)
        {
            Debug.LogError("Could not find 'data' chunk in WAV bytes");
            return null;
        }

        int bytesPerSample = bitsPerSample / 8;
        int sampleCount = dataChunkSize / bytesPerSample;
        float[] samples = new float[sampleCount];

        if (bitsPerSample == 16)
        {
            for (int i = 0; i < sampleCount; i++)
            {
                short raw = BitConverter.ToInt16(wavBytes, dataChunkOffset + i * 2);
                samples[i] = raw / 32768f;
            }
        }
        else if (bitsPerSample == 8)
        {
            for (int i = 0; i < sampleCount; i++)
            {
                samples[i] = (wavBytes[dataChunkOffset + i] - 128) / 128f;
            }
        }
        else
        {
            Debug.LogError($"Unsupported WAV bit depth: {bitsPerSample}");
            return null;
        }

        AudioClip clip = AudioClip.Create(clipName, sampleCount / channels, channels, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static int FindChunk(byte[] data, string chunkId, out int chunkSize)
    {
        int offset = 12; // skip "RIFF____WAVE" header
        while (offset + 8 <= data.Length)
        {
            string id = System.Text.Encoding.ASCII.GetString(data, offset, 4);
            int size = BitConverter.ToInt32(data, offset + 4);

            if (id == chunkId)
            {
                chunkSize = size;
                return offset + 8;
            }

            offset += 8 + size + (size % 2); // chunks are word-aligned
        }

        chunkSize = 0;
        return -1;
    }
}
