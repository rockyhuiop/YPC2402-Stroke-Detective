using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSubtitleController : MonoBehaviour
{
    public GameObject subtitle; // Assign the subtitle GameObject in the Inspector (child of the player)
    private List<GameObject> npcs;
    private TMP_Text subtitleTextMesh; // Assuming TextMesh; adjust if using UI Text or TextMeshPro
    private float distanceThreshold = 2.5f;

    void Start()
    {
        // Find all NPCs in the scene by tag
        npcs = new List<GameObject>(GameObject.FindGameObjectsWithTag("NPC"));
        
        // Get the TextMesh component from the subtitle GameObject
        subtitleTextMesh = subtitle.GetComponentInChildren<TMP_Text>();
        
        // Initially hide the subtitle
        subtitle.SetActive(false);
    }

    void Update()
    {
        float minDistance = float.MaxValue;
        GameObject closestNPC = null;

        // Find the closest NPC within the distance threshold
        foreach (var npc in npcs)
        {
            float distance = Vector3.Distance(transform.position, npc.transform.position);
            Debug.Log("Distance to " + npc.name + ": " + distance);
            if (distance <= distanceThreshold && distance < minDistance)
            {
                minDistance = distance;
                closestNPC = npc;
                Debug.Log("Closest NPC: " + closestNPC.name + " at distance: " + minDistance);
            }
        }

        // Show subtitle for the closest NPC within range, otherwise hide it
        if (closestNPC != null)
        {
            string text = closestNPC.GetComponent<NPC>().subtitleText;
            subtitleTextMesh.text = text;
            subtitle.SetActive(true);
        }
        else
        {
            subtitle.SetActive(false);
        }
    }
}