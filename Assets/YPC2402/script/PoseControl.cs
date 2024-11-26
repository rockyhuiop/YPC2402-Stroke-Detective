using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseControl : MonoBehaviour
{
    public Animator PoseAnimator;
    [SerializeField] string CurrnetState;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //find the animator if it is null (the animator is not apper in the beginning, if set it in start(), it will be missing)
        if ( PoseAnimator == null ) {
            PoseAnimator = GetComponent<Animator>();
        }
        //set the pose
        if (CurrnetState!=""&&!PoseAnimator.GetCurrentAnimatorStateInfo(0).IsName(CurrnetState)) {
            PoseAnimator.Play(CurrnetState);
        }
    }
}
