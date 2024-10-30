using System;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.AIStudio
{
    [RequireComponent(typeof(NPCMovement))]
    public class RPMDialogueCharacterAnimator : MonoBehaviour, IDialogueCharacterAnimator
    {
        public SkinnedMeshRenderer head;
        public Animator animator;
        public float walkingSpeed = 2.5f;

        private List<(string partial, string name, Action<DialogueCharacter, string, string> action)> animations = new List<(string partial, string name, Action<DialogueCharacter, string, string> action)>();

        private void Awake()
        {
            if (animator == null)
                animator = GetComponent<Animator>();

            animations.Add(("smil", "smiling", smile));
            animations.Add(("smirk", "smiling", smile));
            animations.Add(("moves toward", "walking", movesToward));

            GetComponent<NPCMovement>().OnMovement.AddListener(onMovement);
        }

        private void onMovement(bool moving)
        {
            if (moving)
                animator.SetFloat("MoveSpeed", walkingSpeed);
            else
                animator.SetFloat("MoveSpeed", 0);
        }

        private void smile(DialogueCharacter character, string name, string action)
        {
            if (head)
                head.SetBlendShapeWeight(1, 1);
        }

        private void movesToward(DialogueCharacter character, string name, string action)
        {
            NPCMovement movement = character.GetComponent<NPCMovement>();
            string tag = action.Replace("moves toward", "").Trim();

            if (tag != null)
            {
                GameObject destination = GameObject.FindGameObjectWithTag(tag);

                if (destination)
                    movement.Destination = destination.transform;
            }
            else
                Debug.LogWarning($"RPMDialogueCharacterAnimator.movesToward: missing tag {tag}");
        }

        public void AnimateActions(DialogueCharacter character, string[] actions)
        {
            if (actions != null)
            {
                foreach (string action in actions)
                {
                    bool matched = false;

                    foreach (var animation in animations)
                    {
                        if (action.Contains(animation.partial))
                        {
                            animation.action.Invoke(character, animation.name, action);
                            matched = true;
                            break;
                        }
                    }

                    if (!matched)
                        Debug.Log($"RPMCharacterAnimator.AnimateActions unknown action \"{action}\"");
                }
            }
        }

        public void OnEndSpeaking(DialogueCharacter character, DialogueCharacter speaker)
        {
            if (head && head.GetBlendShapeWeight(1) > 0)
                head.SetBlendShapeWeight(1, 0);
        }

        public void OnSpeechRecognized(DialogueCharacter character, DialogueCharacter player)
        {

        }

        public void OnStartSpeaking(DialogueCharacter character, DialogueCharacter speaker)
        {

        }

        public void OnStartSpeechRecognition(DialogueCharacter character, DialogueCharacter player)
        {

        }
    }
}
