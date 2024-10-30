using StarterAssets;
using UnityEngine;

namespace DracarysInteractive.AIStudio
{
    public class ThirdPersonControllerBridge : MonoBehaviour
    {
        public ThirdPersonController tpc;

        private void Awake()
        {
            tpc = GetComponentInParent<ThirdPersonController>();
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            tpc.OnFootstep(animationEvent);
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            tpc.OnLand(animationEvent);
        }
    }
}