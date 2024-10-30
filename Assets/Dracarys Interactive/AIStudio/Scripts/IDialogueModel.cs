using System;
using UnityEngine.Networking;

namespace DracarysInteractive.AIStudio
{
    public interface IDialogueModel
    {
        void Prompt(string prompt);
        void Speak(string text);
        void Respond(string text);
        void Complete(Action<string> onResponse, Action<UnityWebRequest> onError);
        void Clear();
    }
}
