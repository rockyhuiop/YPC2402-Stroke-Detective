using System;
using UnityEngine.Networking;

namespace DracarysInteractive.AIStudio
{
    public class DialogueModel : Pluggable<DialogueModel, IDialogueModel>
    {
        public void Clear()
        {
            Implementation.Clear();
        }

        public void Complete(Action<string> onResponse, Action<UnityWebRequest> onError)
        {
            Implementation.Complete(onResponse, onError);
        }

        public void Prompt(string prompt)
        {
            Implementation.Prompt(prompt);
        }

        public void Speak(string text)
        {
            Implementation.Speak(text);
        }

        public void Respond(string response)
        {
            Implementation.Respond(response);
        }
    }
}