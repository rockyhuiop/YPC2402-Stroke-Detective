using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using DracarysInteractive.AIStudio;

public class FacePlayer : MonoBehaviour
{
    private Transform player;
    private Transform self;
    public bool SharpTurn;
    public bool run;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        self=GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //only run the script if run
        if (!run) {
            return;
        }
        //turn and face player
        float self_rot_adjust=self.rotation.eulerAngles.y%360;
        if (self_rot_adjust < 0) {
            self_rot_adjust=360-self_rot_adjust;
        }
        float target_deg=(Mathf.Atan(math.abs(player.position.z-self.transform.position.z)/math.abs(player.position.x-self.position.x))*Mathf.Rad2Deg)%360;
        if (player.transform.position.z>self.position.z && player.transform.position.x>self.position.x) {
            target_deg=90-target_deg;
        } else if (player.transform.position.z<self.position.z && player.transform.position.x>self.position.x) {
            target_deg=90+target_deg;
        } else if (player.transform.position.z<self.position.z && player.transform.position.x<self.position.x) {
            target_deg=270-target_deg;
        } else if (player.transform.position.z>self.position.z && player.transform.position.x<self.position.x) {
            target_deg=270+target_deg;
        }
        if (!SharpTurn){
            float speed=60.0f;
            if (math.abs(self_rot_adjust-target_deg)>speed*0.8 * Time.deltaTime){
                if (self_rot_adjust>target_deg){
                    if (self_rot_adjust - target_deg < 180) {
                        self.eulerAngles = new Vector3(self.eulerAngles.x,self.eulerAngles.y-speed * Time.deltaTime,self.eulerAngles.z);
                    } else {
                        self.eulerAngles = new Vector3(self.eulerAngles.x,self.eulerAngles.y+speed * Time.deltaTime,self.eulerAngles.z);
                    }
                } else {
                    if (target_deg - self_rot_adjust > 180) {
                        self.eulerAngles = new Vector3(self.eulerAngles.x,self.eulerAngles.y-speed * Time.deltaTime,self.eulerAngles.z);
                    } else {
                        self.eulerAngles = new Vector3(self.eulerAngles.x,self.eulerAngles.y+speed * Time.deltaTime,self.eulerAngles.z);
                    }
                    
                }
            }
        } else {
            self.eulerAngles = new Vector3(self.eulerAngles.x,target_deg+180,self.eulerAngles.z);
        }

    }
}
