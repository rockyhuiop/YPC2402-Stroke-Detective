namespace DracarysInteractive.AIStudio
{
    public interface IDialogueCameraDirector
    {
        void OnStartSpeechRecognition(DialogueCharacter character);
        void OnSpeechRecognized(DialogueCharacter character);
        void OnStartSpeaking(DialogueCharacter character);
        void OnEndSpeaking(DialogueCharacter character);
    }
}
