#if USE_CINEMACHINE
using Cinemachine;
#endif
using UnityEngine;

namespace DracarysInteractive.AIStudio
{
    public class FreeLookCameraDirector : MonoBehaviour, IDialogueCameraDirector
    {
#if USE_CINEMACHINE
        public CinemachineFreeLook _freeLookCamera;

        private CinemachineFreeLook freeLookCamera
        {
            get
            {
                if (!_freeLookCamera)
                {
                    _freeLookCamera = FindObjectOfType<CinemachineFreeLook>();
                }

                return _freeLookCamera;
            }
        }

        public void OnStartSpeaking(DialogueCharacter speaker)
        {
            freeLookCamera.LookAt = speaker.lookAtTarget;
        }

        public void OnEndSpeaking(DialogueCharacter speaker)
        {
            freeLookCamera.LookAt = null;
        }

        public void OnStartSpeechRecognition(DialogueCharacter speaker)
        {
            freeLookCamera.LookAt = speaker.lookAtTarget;
        }

        public void OnSpeechRecognized(DialogueCharacter player)
        {
            freeLookCamera.LookAt = null;
        }
#else
        public void OnEndSpeaking(DialogueCharacter character)
        {
            throw new System.NotImplementedException();
        }

        public void OnSpeechRecognized(DialogueCharacter character)
        {
            throw new System.NotImplementedException();
        }

        public void OnStartSpeaking(DialogueCharacter character)
        {
            throw new System.NotImplementedException();
        }

        public void OnStartSpeechRecognition(DialogueCharacter character)
        {
            throw new System.NotImplementedException();
        }
#endif
    }
}
