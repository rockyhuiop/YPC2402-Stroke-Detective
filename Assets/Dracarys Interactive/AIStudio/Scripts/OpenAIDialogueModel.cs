#if USE_COM_OPENAI_API_UNITY
using OpenAi.Api.V1;
using OpenAi.Unity.V1;
#endif
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace DracarysInteractive.AIStudio
{
    public class OpenAIDialogueModel : MonoBehaviour, IDialogueModel
    {
#if USE_COM_OPENAI_API_UNITY
        public void Clear()
        {
            OpenAiChatCompleterV1.Instance.dialogue.Clear();
        }

        public void Complete(Action<string> onResponse, Action<UnityWebRequest> onError)
        {
            OpenAiChatCompleterV1.Instance.Complete(onResponse, onError);
        }

        public void Prompt(string prompt)
        {
            add(prompt, MessageV1.MessageRole.system);
        }

        public void Speak(string text)
        {
            add(text, MessageV1.MessageRole.user);
        }

        public void Respond(string response)
        {
            add(response, MessageV1.MessageRole.assistant);
        }

        private void add(string content, MessageV1.MessageRole role)
        {
            MessageV1 message = new MessageV1();
            message.content = content;
            message.role = role;
            OpenAiChatCompleterV1.Instance.dialogue.Add(message);
        }
#else
        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Complete(Action<string> onResponse, Action<UnityWebRequest> onError)
        {
            throw new NotImplementedException();
        }

        public void Prompt(string prompt)
        {
            throw new NotImplementedException();
        }

        public void Respond(string text)
        {
            throw new NotImplementedException();
        }

        public void Speak(string text)
        {
            throw new NotImplementedException();
        }
#endif
    }
}