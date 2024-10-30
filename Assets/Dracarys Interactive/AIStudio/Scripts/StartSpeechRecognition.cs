using System;

namespace DracarysInteractive.AIStudio
{
    public class StartSpeechRecognition : DialogueAction<DialogueCharacter>
    {
        public StartSpeechRecognition(DialogueCharacter player, Action onCompletion = null) : base(onCompletion)
        {
            data = player;
        }

        public override void Invoke()
        {
            data.OnStartSpeechRecognition();

            if (onCompletion != null)
                onCompletion.Invoke();
        }
    }
}