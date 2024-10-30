using System;
using System.Threading.Tasks;
using UnityEngine;

namespace DracarysInteractive.AIStudio
{
    [RequireComponent(typeof(ISpeechServices))]
    public class SpeechServices : Pluggable<SpeechServices, ISpeechServices>
    {
        public bool recognizeContinuously = true;
        public GameObject warningText;

        protected override void Awake()
        {
            // Force instantiation of plug-in otherwise we
            // might call GetComponent in a subthread.
            var bootImplementation = Implementation;
        }

        public void StartContinuousRecognizing(Action onStartSpeechRecognition, Action<string> onSpeechRecognized)
        {
            Implementation.StartContinuousRecognizing(onStartSpeechRecognition, onSpeechRecognized);
        }

        public void StopContinuousRecognizing()
        {
            Implementation.StopContinuousRecognizing();
        }

        public void Recognize(Action onStartSpeechRecognition, Action<string> onSpeechRecognized)
        {
            Implementation.Recognize(onStartSpeechRecognition, onSpeechRecognized);
        }
     
        public void Speak(string text, string voice, Action<float[]> onDataReceived, Action onSynthesisCompleted)
        {
            Implementation.Speak(text, voice, onDataReceived, onSynthesisCompleted);
        }
        public float SampleRate()
        {
            return Implementation.SampleRate();
        }

        void Update()
        {
            if (warningText)
            {
                warningText.SetActive(!recognizeContinuously);
            }

            if (!recognizeContinuously && Input.GetKeyDown(KeyCode.Backspace))
            {
                Implementation.Recognize(() =>
                {
                    DialogueActionManager.Instance.EnqueueAction(new StartSpeechRecognition(DialogueManager.Instance.GetPlayer()));
                },
                (string text) =>
                {
                    DialogueActionManager.Instance.EnqueueAction(new SpeechRecognized(DialogueManager.Instance.GetPlayer(), text));
                });
            }
        }
    }
}
