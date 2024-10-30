using UnityEngine;

namespace DracarysInteractive.AIStudio
{
    public abstract class DialogueTrigger : MonoBehaviour
    {
        public DialogueSO dialogue;
        public bool _triggered = false;

        protected void OnTrigger()
        {
            if (!Triggered)
            {
                DialogueManager.Instance.StartDialogue(dialogue);
                Triggered = true;
            }
        }

        public bool Triggered
        {
            get { return _triggered; }
            set { _triggered = value; }
        }
    }
}