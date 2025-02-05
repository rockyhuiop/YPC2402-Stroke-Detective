using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechManager : MonoBehaviour
{
    public static SpeechManager instance;
    [SerializeField] private CognitiveSpeech cognitiveSpeech;
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null){
            instance = this;
        }else{
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SynthesizeSpeech(string text)
    {
        _ = cognitiveSpeech.SynthesizeSpeech(text);
    }
}
