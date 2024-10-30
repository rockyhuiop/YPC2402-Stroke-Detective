using System;
using System.Threading.Tasks;

namespace DracarysInteractive.AIStudio
{
    public interface ISpeechServices
    {
        void StartContinuousRecognizing(Action onStartSpeechRecognition, Action<string> onSpeechRecognized);
        void StopContinuousRecognizing();
        void Recognize(Action onStartSpeechRecognition, Action<string> onSpeechRecognized);
        void Speak(string text, string voice, Action<float[]> onDataReceived, Action onSynthesisCompleted);
        float SampleRate();
    }
}
