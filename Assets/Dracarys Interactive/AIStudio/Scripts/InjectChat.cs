using System;

namespace DracarysInteractive.AIStudio
{
    public class InjectChat : CompleteChat
    {
        public InjectChat(string text, bool prompt = false, Action onCompletion = null) : base(text, prompt, onCompletion)
        {
        }
        public override void Invoke()
        {
            if (!DialogueManager.Instance.HasPlayer)
            {
                if (data.prompt)
                    DialogueModel.Instance.Prompt(data.text);
                else
                    DialogueModel.Instance.Speak(data.text);
            }

            OnChatCompletion(data.text);
        }
    }
}