using UnityEngine;
using cherrydev;

public class DialogController : MonoBehaviour
{
    [SerializeField] private DialogBehaviour dialogBehaviour;
    [SerializeField] private DialogNodeGraph dialogGraph;
    private GameManager gameManager;
    public GameObject correctSign;
    public GameObject wrongSign;

    private void Start()
    {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
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
        gameManager.AddDone(true);
    }

    public void showWrongSign(){
        wrongSign.SetActive(true);
        gameManager.AddDone(false);
    }
}