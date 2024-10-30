using System;
using UnityEngine;

namespace DracarysInteractive.AIStudio
{
    [RequireComponent(typeof(Animator))]
    public class SimpleCharacterAnimator : MonoBehaviour, IDialogueCharacterAnimator
    {
        public void OnStartSpeaking(DialogueCharacter character, DialogueCharacter speaker)
        {
            speaker.GetComponent<Animator>().SetBool("speaking", true);
        }

        public void OnEndSpeaking(DialogueCharacter character, DialogueCharacter speaker)
        {
            speaker.GetComponent<Animator>().SetBool("speaking", false);
        }

        public void OnStartSpeechRecognition(DialogueCharacter character, DialogueCharacter speaker)
        {
                speaker.GetComponent<Animator>().SetBool("speaking", true);
        }

        public void OnSpeechRecognized(DialogueCharacter character, DialogueCharacter speaker)
        {
            speaker.GetComponent<Animator>().SetBool("speaking", false);
        }

        public void AnimateActions(DialogueCharacter character, string[] actions)
        {
        }
    }
}
