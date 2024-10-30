using System;

namespace DracarysInteractive.AIStudio
{
    [Serializable]
    public abstract class DialogueAction<T> : DialogueActionBase
    {
        private T _data;

        protected DialogueAction(Action onCompletion = null) : base(onCompletion)
        {

        }

        public T data
        {
            get { return _data; }
            protected set { _data = value; }
        }
    }
}
