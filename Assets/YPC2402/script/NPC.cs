using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public string subtitleText; // Set this in the Unity Inspector for each NPC
    public GameObject subtitleObject;
    public TMP_Text subtitleTextMesh; // Assuming TextMesh; adjust if using UI Text or TextMeshPro
    public float subtitleRadius = 1.0f; // Radius of the circle around the NPC
    public float subtitleHeightOffset = 1.5f; // Height above the NPC for the subtitle
}