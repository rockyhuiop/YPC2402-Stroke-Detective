using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public string subtitleText; // Set this in the Unity Inspector for each NPC
    public GameObject subtitleObject;
    public TMP_Text subtitleTextMesh; // Assuming TextMesh; adjust if using UI Text or TextMeshPro
    public float subtitleRadius = 1.0f; // Radius of the circle around the NPC
    public float subtitleHeightOffset = 1.5f; // Height above the NPC for the subtitle
    public StrokeDetectiveNPCData NPCData;
    public bool isMale;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Transform>().Find("AIChatCore").gameObject.SetActive(!GameObject.FindObjectOfType<GlobalGameManager>().Easy);
        GetComponent<Transform>().Find("Dialog Prefab").gameObject.SetActive(GameObject.FindObjectOfType<GlobalGameManager>().Easy);
        GetComponent<Transform>().Find("Dialog Controller").gameObject.SetActive(GameObject.FindObjectOfType<GlobalGameManager>().Easy);
        GetComponent<Transform>().Find("ChatGPT Dialogue").gameObject.SetActive(GameObject.FindObjectOfType<GlobalGameManager>().Easy);
    }
}