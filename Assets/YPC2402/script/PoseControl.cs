using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseControl : MonoBehaviour
{
    public Animator PoseAnimator;
    //current pose
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
        //if current state not set
        if (CurrnetState!=""&&!PoseAnimator.GetCurrentAnimatorStateInfo(0).IsName(CurrnetState)) {
            //set the pose
            PoseAnimator.Play(CurrnetState);
            //update the collider (collider need to update manually if pose changed)
            GetComponent<SkinnedCollider>().UpdateCollider();
        }
    }
}
