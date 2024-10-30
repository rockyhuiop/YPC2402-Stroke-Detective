using UnityEngine;

namespace DracarysInteractive.AIStudio
{
    public class ProximityTrigger : DialogueTrigger
    {
        public Transform go1, go2;
        public float triggerDistance = 1f;
        public bool triggerNear = true;

        void Update()
        {
            float distance = Vector3.Distance(go1.position, go2.position);

            if ((triggerNear && distance < triggerDistance) || (!triggerNear && distance > triggerDistance))
                OnTrigger();
            else
                Triggered = false;
        }
    }
}