using UnityEngine;
using cherrydev;

public class DialogController : MonoBehaviour
{
    [SerializeField] private DialogBehaviour dialogBehaviour;
    [SerializeField] private DialogNodeGraph dialogGraph;
    private GameManager gameManager;
    public GameObject correctSign;
    public GameObject wrongSign;

    public Transform NPC;

    bool hasStarted = false;

    private void Start()
    {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
        dialogBehaviour.BindExternalFunction("Test", DebugExternal);
        dialogBehaviour.BindExternalFunction("showCorrectSign", showCorrectSign);
        dialogBehaviour.BindExternalFunction("showWrongSign", showWrongSign);
        dialogBehaviour.BindExternalFunction("FullySmile", FullySmile);
        dialogBehaviour.BindExternalFunction("HalfSmile", HalfSmile);
        dialogBehaviour.BindExternalFunction("LiftHand", LiftHand);
        dialogBehaviour.BindExternalFunction("LiftHandFail", LiftHandFail);

        //dialogBehaviour.StartDialog(dialogGraph);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasStarted) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            dialogBehaviour.StartDialog(dialogGraph);
            hasStarted = true;
        }
    }

    public void FullySmile()
    {
        ExpressionControl expressionControl = NPC.GetComponent<ExpressionControl>();
        expressionControl.FullySmile();
    }

    public void HalfSmile()
    {
        bool isLeft = Random.Range(0, 2) == 0;
        ExpressionControl expressionControl = NPC.GetComponent<ExpressionControl>();
        expressionControl.HalfSmile(isLeft);
    }

    public void LiftHand(){
        PoseControl poseControl = NPC.GetComponent<PoseControl>();
        string pose = poseControl.GetPose();
        if(pose.Contains("sit")){
            poseControl.SetPose("liftarm_sit");
        }else if(pose.Contains("stand")){
            poseControl.SetPose("liftarm_stand");
        }else if(pose.Contains("liedown")){
            poseControl.SetPose("liftarm_liedown");
        }
    }

    public void LiftHandFail(){
        PoseControl poseControl = NPC.GetComponent<PoseControl>();
        string pose = poseControl.GetPose();
        if(pose.Contains("sit")){
            poseControl.SetPose("liftarm_sit_half");
        }else if(pose.Contains("stand")){
            poseControl.SetPose("liftarm_stand_half");
        }else if(pose.Contains("liedown")){
            poseControl.SetPose("liftarm_liedown_half");
        }
    }


    private void DebugExternal()
    {
        Debug.Log("External function works!");
    }

    
    public void showCorrectSign(){
        correctSign.SetActive(true);
        gameManager.AddDone(true);
    }

    public void showWrongSign(){
        wrongSign.SetActive(true);
        gameManager.AddDone(false);
    }
}