using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : MonoBehaviour
{
    [SerializeField] Animator animator;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (animator == null) {
            animator = GetComponent<Animator>();
            
        } else {
            animator.enabled=false;
        }
    }
}
