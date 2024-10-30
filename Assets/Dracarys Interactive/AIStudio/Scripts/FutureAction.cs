using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace DracarysInteractive.AIStudio
{
    public class FutureAction : DialogueAction<(DialogueActionBase action, float time)>
    {
        public FutureAction(DialogueActionBase action, float delay, Action onCompletion = null) : base(onCompletion)
        {
            data = (action, DialogueActionManager.Instance.currentTime + delay);
        }

        public override void Invoke()
        {
            if (onCompletion != null)
                onCompletion.Invoke();

            if (DialogueActionManager.Instance.currentTime < data.time)
            {
                DialogueActionManager.Instance.EnqueueAction(this);
            }
            else
            {
                DialogueActionManager.Instance.EnqueueAction(data.action);
            }
        }
    }
}