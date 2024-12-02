using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseControl : MonoBehaviour
{
    private Animator PoseAnimator;
    //current pose
    [SerializeField] string CurrnetState;
    bool animation_end = false;
    bool animation_start = false;
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
        if (CurrnetState!=""&&!PoseAnimator.GetCurrentAnimatorStateInfo(0).IsName(CurrnetState)&&!animation_end&&!animation_start) {
            //set the pose
            StartCoroutine(SetPoseCorr(CurrnetState));
        }
    }
    private AnimationClip FindAnimation(Animator animator, string name)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }

        return null;
    }
    public void SetPose(string state) {
        //set current animation state
        CurrnetState = state;
        //reset the variable for recording animation playing status (in order to let the update run the animation again)
        animation_end=false;
        animation_start=false;
    }

    public string GetPose(){
        return CurrnetState;
    }
    IEnumerator SetPoseCorr(string state){
        animation_start=true;
        //play the animation
        PoseAnimator.Play(state);
        //wait for the animation end
        yield return new WaitForSeconds(FindAnimation(PoseAnimator, state).length);
        animation_end=true;
        //update the collider (collider need to update manually if pose changed)
        GetComponent<SkinnedCollider>().ColliderSet=false;
    }
}
