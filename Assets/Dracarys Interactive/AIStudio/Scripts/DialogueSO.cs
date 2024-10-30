using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.AIStudio
{
    [CreateAssetMenu]
    public class DialogueSO : ScriptableObject
    {
        [TextArea(10, 100)]
        public string dialogueContext;

        public CharacterSO player;

        public List<CharacterSO> nonPlayerCharacters;

        [TextArea(5, 100)]
        public string initialPrompt;

        [TextArea(5, 100)]
        public string closingPrompt;

        public float closingPromptDelay;

        public string nextScene;
    }
}
