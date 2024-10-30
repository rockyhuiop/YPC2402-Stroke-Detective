using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.AIStudio
{
    /// <summary>
    /// Add this component if rare issue arises where character audio,
    /// which by default is 3D, degrades over time. A hack but 
    /// not interested in investigating further since it only
    /// happens on cheapo discount hardware.
    /// </summary>
    public class SpatialBlendEmulator : MonoBehaviour
    {
        public float dampening = 1.5f;

        private AudioListener _listener;
        private AudioSource[] _sources;

        void Awake()
        {
            _listener = FindObjectOfType<AudioListener>();
            _sources = FindObjectsOfType<AudioSource>();

            foreach (AudioSource source in _sources)
            {
                source.spatialBlend = 0;
            }
        }

        void Update()
        {
            foreach (AudioSource source in _sources)
            {
                float distance = Vector3.Distance(_listener.gameObject.transform.position, source.gameObject.transform.position);
                source.volume = 1 / (distance * dampening);
            }
        }
    }
}
