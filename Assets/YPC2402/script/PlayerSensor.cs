using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    private NavAI navAI;
    public List<GameObject> colliderList = new List<GameObject>();
    private void Start() {
        navAI = GetComponent<Transform>().parent.GetComponent<NavAI>();
    }
    public void OnTriggerEnter(Collider collider)
    {
        if (!colliderList.Contains(collider.gameObject))
        {
            colliderList.Add(collider.gameObject);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (colliderList.Contains(collider.gameObject))
        {
            colliderList.Remove(collider.gameObject);
        }
    }

    public List<GameObject> AllGameObject()
    {
        return colliderList;
    }
    private void Update() {
        bool havePlayer=false;
        Transform player=null;
        Transform NPC=null;
        foreach (var collider in colliderList) {
            if (collider.transform.CompareTag("Player")) {
                player = collider.transform;
                NPC = GetComponent<Transform>().parent;
                Debug.Log("player: "+collider.transform.rotation.eulerAngles.y+" NPC: "+GetComponent<Transform>().parent.rotation.eulerAngles.y+" diff: "+math.abs(player.rotation.eulerAngles.y - NPC.rotation.eulerAngles.y-180) );
                havePlayer=true;
                break;
            }          
        }
        navAI.force_stop=havePlayer;
        if (havePlayer) {
            if ((player.rotation.eulerAngles.y - NPC.rotation.eulerAngles.y-180)>0.5){
                NPC.eulerAngles = new Vector3(NPC.eulerAngles.x,NPC.eulerAngles.y-5.0f * Time.deltaTime,NPC.eulerAngles.z);
            } else if ((player.rotation.eulerAngles.y - NPC.rotation.eulerAngles.y - 180) < 0.5){
                NPC.eulerAngles = new Vector3(NPC.eulerAngles.x,NPC.eulerAngles.y+5.0f * Time.deltaTime,NPC.eulerAngles.z);
            }
        }
    }
}
