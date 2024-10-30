namespace DracarysInteractive.AIStudio
{
    public abstract class Pluggable<T, I> : Singleton<T>
        where T : Pluggable<T, I>
        where I : class
    {
        private static I _implementation;

        /// <summary>
        /// Property returning the instance as the interface type.
        /// </summary>
        public static I Implementation
        {
            get
            {
                if (_implementation == null)
                {
                    _implementation = Instance.GetComponent<I>();
                }

                return _implementation;
            }
        }
    }
}
