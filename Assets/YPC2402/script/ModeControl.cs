using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeControl : MonoBehaviour
{
    [SerializeField] GameObject[] hardAI;
    [SerializeField] GameObject[] easyAI;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var obj in hardAI) {
            obj.gameObject.SetActive(!GameObject.FindObjectOfType<GlobalGameManager>().Easy);
        }
        foreach (var obj in easyAI) {
            obj.gameObject.SetActive(GameObject.FindObjectOfType<GlobalGameManager>().Easy);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
