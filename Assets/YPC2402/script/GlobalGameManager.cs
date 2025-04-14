using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalGameManager : MonoBehaviour
{
    private static GlobalGameManager _instance;
    public bool Easy;
    public GlobalGameManager GlobalGameManagerInstance{
        get { return _instance; }   
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Debug.Log(SceneManager.GetActiveScene().name);
    }
    private void Awake() {
        if (_instance != null) {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleEasy() {
        Easy=!Easy;
    }
    public bool GetEasy() {
        return Easy;
    }
}
