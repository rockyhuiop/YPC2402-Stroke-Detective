using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSubtitleController : MonoBehaviour
{
    public static PlayerSubtitleController instance;
    public GameObject subtitle; // Assign the subtitle GameObject in the Inspector (child of the player)
    private List<GameObject> npcs;
    private TMP_Text subtitleTextMesh; // Assuming TextMesh; adjust if using UI Text or TextMeshPro
    private float distanceThreshold = 2.5f;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        // Find all NPCs in the scene by tag
        npcs = new List<GameObject>(GameObject.FindGameObjectsWithTag("NPC"));
        
        // Get the TextMesh component from the subtitle GameObject
        subtitleTextMesh = subtitle.GetComponentInChildren<TMP_Text>();
        
        // Initially hide the subtitle
        subtitle.SetActive(false);

        foreach (var npc in npcs)
        {
            // Assuming each NPC has a SubtitleText component with the text to display
            var npcComponent = npc.GetComponent<NPC>();
            if (npcComponent != null)
            {
                // Set the subtitle text for each NPC (optional, can be set in the Inspector)
                //npcComponent.subtitleText = "Hello! I'm " + npc.name; // Example text
                npcComponent.subtitleObject.SetActive(false); // Hide the NPC's subtitle object initially
            }
        }
    }

    void Update()
    {
        float minDistance = float.MaxValue;
        GameObject closestNPC = null;
        

        // Find the closest NPC within the distance threshold
        foreach (var npc in npcs)
        {
            
            float distance = Vector3.Distance(transform.position, npc.transform.position);
            if (distance <= distanceThreshold && distance < minDistance)
            {
                minDistance = distance;
                closestNPC = npc;
                
            }
        }

        // Show subtitle for the closest NPC within range, otherwise hide it
        if (closestNPC != null)
        {
            Vector3 npcPos = closestNPC.transform.position; // NPC center (C)
            Vector3 playerPos = transform.position; // Player position (P)
            Vector3 directionXZ = new Vector3(playerPos.x - npcPos.x, 0, playerPos.z - npcPos.z);
            Vector3 direction = directionXZ.sqrMagnitude > 0.0001f ? directionXZ.normalized : Vector3.right;

            // Compute subtitle position: center + radius * direction + height offset
            Vector3 subtitlePos = npcPos + closestNPC.GetComponent<NPC>().subtitleRadius * direction + new Vector3(0, closestNPC.GetComponent<NPC>().subtitleHeightOffset, 0);
            //string text = closestNPC.GetComponent<NPC>().subtitleText;
            //subtitleTextMesh.text = text;
            subtitle.SetActive(true);
            closestNPC.GetComponent<NPC>().subtitleObject.SetActive(true); // Show the NPC's subtitle object
            //closestNPC.GetComponent<NPC>().subtitleTextMesh.text = text; // Update the NPC's subtitle text
            if (closestNPC.GetComponent<NPC>().subtitleObject.activeSelf)
            {
                closestNPC.GetComponent<NPC>().subtitleObject.transform.rotation = Camera.main.transform.rotation;
                closestNPC.GetComponent<NPC>().subtitleObject.transform.position = subtitlePos;
            }

        }
        else
        {
            subtitle.SetActive(false);
            closestNPC?.GetComponent<NPC>().subtitleObject.SetActive(false); // Hide the NPC's subtitle object
        }

        
    }
}