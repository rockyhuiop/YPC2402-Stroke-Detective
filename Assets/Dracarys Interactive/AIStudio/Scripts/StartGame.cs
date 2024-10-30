using UnityEngine;
using UnityEngine.SceneManagement;

namespace DracarysInteractive.AIStudio
{
    public class StartGame : MonoBehaviour
    {
        public float delay = 4.5f;
        public AudioSource sfx;
        public float sfxDelay = 2.5f;

        // Start is called before the first frame update
        void Start()
        {
            Invoke("NextScene", delay);
            if (sfx)
                sfx.PlayDelayed(sfxDelay);
        }

        void NextScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}