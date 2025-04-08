using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAI : MonoBehaviour
{
    [SerializeField] PoseControl Pose;
    public enum AIState { Idle, Walking }
    public AIState currentState = AIState.Idle;
    public float walkingSpeed = 0.5f;
    public Animator animator;
    NavMeshAgent agent;
    List<Vector3> previousIdlePoints = new List<Vector3>();
    private float time_to_resume=0; 
    private bool setTimer=false;
    public float speed=0;
    public bool force_stop=false;
    [SerializeField] string scene;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<Transform>().parent.parent.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0.01f;
        agent.autoBraking = !(scene=="City");

        currentState = AIState.Idle;
        Pose=GetComponent<PoseControl>();
        SwitchAnimationState(currentState);
    }
    // Update is called once per frame
    void Update()
    {
        if (force_stop) {
            SwitchAnimationState(AIState.Idle);
            agent.enabled = false;
        } else {
            agent.enabled = true;
        }
        speed=agent.velocity.sqrMagnitude;
        Pose.animIDMotionSpeed=agent.velocity.sqrMagnitude*20.0f;
        if (currentState == AIState.Idle)
        {

            if (agent.isOnNavMesh)
            {
                //stand and wait for 5s
                SwitchAnimationState(currentState);
                if (!setTimer) {
                    time_to_resume=Time.time+5.0f;    
                    setTimer = true;
                }
                if (scene != "City") {
                    if (Time.time < time_to_resume) {
                        return;
                    }
                }
                setTimer =false;
                if (scene == "City") {
                    agent.destination = RandomNavSphere(transform.position, Random.Range(15f, 15f));
                } else {
                    agent.destination = RandomNavSphere(transform.position, Random.Range(2.0f, 3.0f));
                }
                currentState = AIState.Walking;
                SwitchAnimationState(currentState);
                        


                previousIdlePoints.Add(transform.position);
                if (previousIdlePoints.Count > 5)
                {
                    previousIdlePoints.RemoveAt(0);
                }
                        
            }
        }
        else if (currentState == AIState.Walking)
        {
            agent.speed = walkingSpeed;

            if (DoneReachingDestination())
            {
                currentState = AIState.Idle;
                
            }
        }
            
    }

    bool DoneReachingDestination()
    {
        if (!agent.isOnNavMesh)
        {
            return false;
        }
            
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0.01f)
                {
                    return true;
                        
                }
            }
        }


            

        return false;
    }



    public void SwitchAnimationState(AIState state)
    {
        if (state == AIState.Walking) {
            Pose.SetPose("Idle Walk Run Blend");
        } else {
            Pose.SetPose("stand");
        }
        //if (animator)
        //{
        //    animator.SetBool("isWalking", state == AIState.Walking);
        //}
    }


    Vector3 RandomNavSphere(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, NavMesh.AllAreas);

        return navHit.position;
    }

}

