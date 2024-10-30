namespace DracarysInteractive.AIStudio
{
    public interface IDialogueCharacterAnimator
    {
        void AnimateActions(DialogueCharacter character, string[] actions);
        void OnStartSpeechRecognition(DialogueCharacter character, DialogueCharacter player);
        void OnSpeechRecognized(DialogueCharacter character, DialogueCharacter player);
        void OnStartSpeaking(DialogueCharacter character, DialogueCharacter speaker);
        void OnEndSpeaking(DialogueCharacter character, DialogueCharacter speaker);
    }
}
