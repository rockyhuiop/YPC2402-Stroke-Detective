using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PoseControl : MonoBehaviour
{
    private Animator PoseAnimator;
    //current pose
    [SerializeField] string CurrnetState;
    bool animation_end = false;
    bool animation_start = false;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;
    //animation variable
    public float animIDSpeed=1.0f;
    public bool animIDGrounded;
    public bool animIDJump;
    public bool animIDFreeFall;
    public float animIDMotionSpeed=2.0f;
    // Start is called before the first frame update
    void Start()
    {
        AssignAnimationIDs();
    }

    // Update is called once per frame
    void Update()
    {
        Transform y_adjustment = GetComponent<Transform>().parent;
        if (CurrnetState == "stand" || CurrnetState == "Idle Walk Run Blend") {
            y_adjustment.localPosition=new Vector3(0,0.159f,0);
        } else if (CurrnetState == "liedown") {
            y_adjustment.localPosition=new Vector3(0,0.005f,0);
        } else if (CurrnetState == "sit") {
            y_adjustment.localPosition=new Vector3(0,0,0);
        }
        //find the animator if it is null (the animator is not apper in the beginning, if set it in start(), it will be missing)
        if ( PoseAnimator == null ) {
            PoseAnimator = GetComponent<Animator>();
        }
        //if current state not set
        if (CurrnetState!=""&&!PoseAnimator.GetCurrentAnimatorStateInfo(0).IsName(CurrnetState)&&!animation_end&&!animation_start) {
            //set the pose
            StartCoroutine(SetPoseCorr(CurrnetState));
        }
        PoseAnimator.SetFloat(_animIDSpeed, animIDSpeed);
        PoseAnimator.SetFloat(_animIDMotionSpeed, animIDMotionSpeed);
        if (CurrnetState == "liedown") {
            GetComponent<NavAI>().enabled = false;
            GetComponent<Transform>().parent.parent.GetComponent<NavMeshAgent>().enabled = false;
        }
    }
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }
    private float FindAnimationLength(Animator animator, string name)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return clip.length;
            }
        }

        return 0;
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
        yield return new WaitForSeconds(FindAnimationLength(PoseAnimator, state));
        animation_end=true;
        //update the collider (collider need to update manually if pose changed)
        GetComponent<SkinnedCollider>().ColliderSet=false;
    }
    public void OnFootstep(AnimationEvent animationEvent)
    {
        
    }

    public void OnLand(AnimationEvent animationEvent)
    {
        
    }
}
