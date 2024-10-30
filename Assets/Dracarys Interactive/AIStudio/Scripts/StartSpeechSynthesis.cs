using System;

namespace DracarysInteractive.AIStudio
{
    public class StartSpeechSynthesis : DialogueAction<(DialogueCharacter character, string text, string[] actions)>
    {
        public StartSpeechSynthesis((DialogueCharacter character, string text, string[] actions) data, Action onCompletion = null) : base(onCompletion)
        {
            this.data = data;
        }

        public override void Invoke()
        {
            data.character.OnStartSpeaking(data.text, data.actions);

            if (onCompletion != null)
                onCompletion.Invoke();
        }
    }
}