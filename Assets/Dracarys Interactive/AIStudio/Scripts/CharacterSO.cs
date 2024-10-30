using UnityEngine;

namespace DracarysInteractive.AIStudio
{
    [CreateAssetMenu]
    public class CharacterSO : ScriptableObject
    {
        public string character;
        public string voice;
        public string avatar;
        [TextArea(10, 100)]
        public string narrative;
    }
}

