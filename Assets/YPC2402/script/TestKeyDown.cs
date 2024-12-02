using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestKeyDown : MonoBehaviour
{
    public TMP_Text m_text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var allKeys = System.Enum.GetValues(typeof(KeyCode));
        foreach (KeyCode key in allKeys)
        {
            if (Input.GetKeyDown(key))
            {
                m_text.text = key.ToString();
            }
        }
    }
}
