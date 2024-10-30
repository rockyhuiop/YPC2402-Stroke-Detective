using System;

namespace DracarysInteractive.AIStudio
{
    public class SpeechRecognized : DialogueAction<(DialogueCharacter player, string text)>
    {
        public SpeechRecognized(DialogueCharacter player, string text, Action onCompletion = null) : base(onCompletion)
        {
            data = (player, text);
        }

        public override void Invoke()
        {
            data.player.OnSpeechRecognized(data.text);

            if (onCompletion != null)
                onCompletion.Invoke();
        }
    }
}