using System;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;

public class CognitiveSpeech : MonoBehaviour
{
    [Header("Azure Speech Settings")]
    [Tooltip("Your Azure Speech Service Subscription Key")]
    private string subscriptionKey = "45c14d10d2504f31a55a74bc5a6bd0d7";

    [Tooltip("Your Azure Speech Service Region (e.g., westus)")]
    public string region = "eastasia";

    private SpeechConfig speechConfig;

    public enum LanguageCode
    {
        English,
        Chinese,
        Cantonese
    }

    public LanguageCode languageCode = LanguageCode.English;

    void Start()
    {
        // Initialize the Speech Config
        speechConfig = SpeechConfig.FromSubscription(subscriptionKey, region);
        
        // Optionally, set the desired voice
        // You can find available voices here: https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#text-to-speech
        string voiceName = "";
        switch (languageCode)
        {
            case LanguageCode.English:
                //voiceName = "en-GB-AdaMultilingualNeural";
                voiceName = "en-US-AndrewMultilingualNeural";
                break;
            case LanguageCode.Chinese:
                voiceName = "zh-CN-XiaoxiaoMultilingualNeural";
                break;
            case LanguageCode.Cantonese:
                voiceName = "zh-HK-HiuMaanNeural";
                break;
        }
        speechConfig.SpeechSynthesisVoiceName = voiceName;
    }

    

    // Public method to convert text to speech
    public async Task SynthesizeSpeech(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("Text is null or empty. Cannot synthesize speech.");
            return;
        }

        using (var synthesizer = new SpeechSynthesizer(speechConfig, null))
        {
            Debug.Log("Synthesizing speech for text: " + text);
            var result = await synthesizer.SpeakTextAsync(text);

            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                Debug.Log("Speech synthesized successfully.");
                
                // Play the audio using Unity's AudioSource
                PlayAudio(result.AudioData);
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                Debug.LogError($"Speech synthesis canceled: {cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Debug.LogError($"Error details: {cancellation.ErrorDetails}");
                }
            }
        }
    }

    // Method to play audio from byte array
    private void PlayAudio(byte[] audioData)
    {
        // Create an AudioClip from the audio data
        // Azure returns audio in WAV format by default
        // You may need to parse the WAV header

        // For simplicity, consider using Unity's built-in audio playback using WWW or UnityWebRequest
        StartCoroutine(PlayWav(audioData));
    }

    private System.Collections.IEnumerator PlayWav(byte[] wav)
    {
        string tempFilePath = System.IO.Path.Combine(Application.temporaryCachePath, "tempAudio.wav");
        System.IO.File.WriteAllBytes(tempFilePath, wav);

        // Load the WAV file
        using (var www = new WWW("file://" + tempFilePath))
        {
            yield return www;

            AudioClip clip = www.GetAudioClip(false, false, AudioType.WAV);
            if (clip.loadState == AudioDataLoadState.Loaded)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError("Failed to load audio clip from WAV data.");
            }
        }
    }
}