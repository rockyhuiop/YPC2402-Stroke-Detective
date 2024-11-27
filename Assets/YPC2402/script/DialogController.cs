using UnityEngine;
using cherrydev;

public class DialogController : MonoBehaviour
{
    [SerializeField] private DialogBehaviour dialogBehaviour;
    [SerializeField] private DialogNodeGraph dialogGraph;
    public GameObject correctSign;
    public GameObject wrongSign;

    private void Start()
    {
        dialogBehaviour.BindExternalFunction("Test", DebugExternal);
        dialogBehaviour.BindExternalFunction("showCorrectSign", showCorrectSign);
        dialogBehaviour.BindExternalFunction("showWrongSign", showWrongSign);

        dialogBehaviour.StartDialog(dialogGraph);
    }

    private void DebugExternal()
    {
        Debug.Log("External function works!");
    }

    
    public void showCorrectSign(){
        correctSign.SetActive(true);
    }

    public void showWrongSign(){
        wrongSign.SetActive(true);
    }
}