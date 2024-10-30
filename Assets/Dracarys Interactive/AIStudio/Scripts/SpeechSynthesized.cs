using System;

namespace DracarysInteractive.AIStudio
{
    public class SpeechSynthesized : DialogueAction<DialogueCharacter>
    {
        public SpeechSynthesized(DialogueCharacter player, Action onCompletion = null) : base(onCompletion)
        {
            data = player;
        }

        public override void Invoke()
        {
            data.OnEndSpeaking();

            if (onCompletion != null)
                onCompletion.Invoke();
        }
    }
}