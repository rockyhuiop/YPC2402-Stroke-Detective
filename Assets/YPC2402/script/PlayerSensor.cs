using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
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
        Transform NPC=GetComponent<Transform>().parent;
        foreach (var collider in colliderList) {
            if (collider.transform.CompareTag("Player")) {
                player = collider.transform;
                havePlayer=true;
                break;
            }          
        }
        if (NPC.GetComponent<PoseControl>().GetPose()!="stand"){
            return;
        }
        //stop NPC nav ai moving
        navAI.force_stop=havePlayer;
        //turn and face player
        if (havePlayer) {
            float NPC_rot_adjust=NPC.rotation.eulerAngles.y%360;
            if (NPC_rot_adjust < 0) {
                NPC_rot_adjust=360-NPC_rot_adjust;
            }
            float target_deg=(Mathf.Atan(math.abs(player.transform.position.z-NPC.transform.position.z)/math.abs(player.transform.position.x-NPC.transform.position.x))*Mathf.Rad2Deg)%360;
            if (player.transform.position.z>NPC.transform.position.z && player.transform.position.x>NPC.transform.position.x) {
                target_deg=90-target_deg;
            } else if (player.transform.position.z<NPC.transform.position.z && player.transform.position.x>NPC.transform.position.x) {
                target_deg=90+target_deg;
            } else if (player.transform.position.z<NPC.transform.position.z && player.transform.position.x<NPC.transform.position.x) {
                target_deg=270-target_deg;
            } else if (player.transform.position.z>NPC.transform.position.z && player.transform.position.x<NPC.transform.position.x) {
                target_deg=270+target_deg;
            }
            //Debug.Log("player: "+player.transform.position.z+" " + player.transform.position.x +" NPC: "+NPC.position.z +" "+NPC.position.x+" diff: "+(target_deg)+" "+NPC.eulerAngles.y );
            float speed=60.0f;
            if (math.abs(NPC_rot_adjust-target_deg)>speed*0.8 * Time.deltaTime){
                if (NPC_rot_adjust>target_deg){
                    if (NPC_rot_adjust - target_deg < 180) {
                        NPC.eulerAngles = new Vector3(NPC.eulerAngles.x,NPC.eulerAngles.y-speed * Time.deltaTime,NPC.eulerAngles.z);
                    } else {
                        NPC.eulerAngles = new Vector3(NPC.eulerAngles.x,NPC.eulerAngles.y+speed * Time.deltaTime,NPC.eulerAngles.z);
                    }
                } else {
                    if (target_deg - NPC_rot_adjust > 180) {
                        NPC.eulerAngles = new Vector3(NPC.eulerAngles.x,NPC.eulerAngles.y-speed * Time.deltaTime,NPC.eulerAngles.z);
                    } else {
                        NPC.eulerAngles = new Vector3(NPC.eulerAngles.x,NPC.eulerAngles.y+speed * Time.deltaTime,NPC.eulerAngles.z);
                    }
                    
                }
            }
        }
    }
}
