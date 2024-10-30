#if CMS_SALSA
using CrazyMinnow.SALSA;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DracarysInteractive.AIStudio
{
    [RequireComponent(typeof(Animator))]
#if CMS_SALSA
    [RequireComponent(typeof(Eyes))]
    [RequireComponent(typeof(Emoter))]
#endif
    public class UMASALSADialogueCharacterAnimator : MonoBehaviour, IDialogueCharacterAnimator
    {
        public float emoteDuration = 4f;
        public int talkingAnimations = 3;
        public float probabilityOfTalkingAnimation = .2f;

        private List<(string partial, string name, Action<DialogueCharacter, string, string> action)> _animations = new List<(string partial, string name, Action<DialogueCharacter, string, string> action)>();
        private bool _moving = false;

        private void Awake()
        {
            _animations.Add(("smil", "smiling", emote));
            _animations.Add(("smirk", "smiling", emote));
            _animations.Add(("moves toward", "walking", movesToward));
            _animations.Add(("nod", "nodding", trigger));
            _animations.Add(("excit", "talking", talking));
            _animations.Add(("enthusias", "talking", talking));

            if (GetComponent<NPCMovement>())
                GetComponent<NPCMovement>().OnMovement.AddListener(onMovement);
        }

        private void onMovement(bool moving)
        {
            if (_moving = moving)
                trigger(GetComponent<DialogueCharacter>(), "walking", "walking");
            else
                trigger(GetComponent<DialogueCharacter>(), "idle", "idle");
        }

        private void talking(DialogueCharacter character, string name, string action)
        {
            trigger(character, name + Random.Range(1, talkingAnimations + 1), action);
        }

        private void trigger(DialogueCharacter character, string name, string action)
        {
            character.GetComponent<Animator>().SetTrigger(name);
        }

        private void movesToward(DialogueCharacter character, string name, string action)
        {
            NPCMovement movement = character.GetComponent<NPCMovement>();
            string tag = action.Replace("moves toward", "").Trim();

            if (movement && tag != null)
            {
                GameObject destination = GameObject.FindGameObjectWithTag(tag);

                if (destination)
                    movement.Destination = destination.transform;
            }
            else
                Debug.LogWarning($"UMASALSADialogueCharacterAnimator.movesToward: missing tag {tag}");
        }

        private void emote(DialogueCharacter character, string name, string action)
        {
#if CMS_SALSA
            character.GetComponent<Emoter>().ManualEmote(name, ExpressionComponent.ExpressionHandler.RoundTrip, emoteDuration);
#endif
        }

        public void AnimateActions(DialogueCharacter character, string[] actions)
        {
            if (!_moving && actions != null)
            {
                foreach (string action in actions)
                {
                    foreach (var animation in _animations)
                    {
                        if (action.Contains(animation.partial))
                        {
                            animation.action.Invoke(character, animation.name, action);
                            break;
                        }
                    }
                }
            }
        }

        public void OnStartSpeaking(DialogueCharacter character, DialogueCharacter speaker)
        {
#if CMS_SALSA
            Eyes eyes = character.GetComponent<Eyes>();

            if (character == speaker)
            {
                eyes.lookTarget = null;
                eyes.LookForward();
                
                if (Random.value < probabilityOfTalkingAnimation)
                    talking(character, "talking", "talking");
            }
            else
            {
                eyes.lookTarget = speaker.lookAtTarget;
            }
#endif
        }

        public void OnEndSpeaking(DialogueCharacter character, DialogueCharacter speaker)
        {
#if CMS_SALSA
            Eyes eyes = character.GetComponent<Eyes>();

            eyes.lookTarget = null;
            eyes.LookForward();
#endif
        }

        public void OnStartSpeechRecognition(DialogueCharacter character, DialogueCharacter speaker)
        {
#if CMS_SALSA
            Eyes eyes = character.GetComponent<Eyes>();

            if (character == speaker)
            {
                eyes.lookTarget = null;
                eyes.LookForward();
            }
            else
            {
                eyes.lookTarget = speaker.lookAtTarget;
            }
#endif
        }

        public void OnSpeechRecognized(DialogueCharacter character, DialogueCharacter player)
        {
#if CMS_SALSA
            if (character == player)
            {
                Eyes eyes = player.GetComponent<Eyes>();

                eyes.lookTarget = null;
                eyes.LookForward();
            }
#endif
        }
    }
}
