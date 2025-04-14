using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetEasy : MonoBehaviour
{
    public Toggle toggle;
    public GlobalGameManager globalGameManager;
    // Start is called before the first frame update
    void Start()
    {
        toggle=GetComponent<Toggle>();
        globalGameManager=GameObject.FindObjectOfType<GlobalGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Sync() {
        globalGameManager.Easy=toggle.isOn;
    }
}
