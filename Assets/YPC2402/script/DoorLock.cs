using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLock : MonoBehaviour
{
    Transform self;
    public bool handle_down = false;
    public bool lock_area = true;
    [SerializeField] Transform Handler;
    [SerializeField] GameObject LockCollider;
    public HingeJoint hinge;
    JointLimits limits;
    // Start is called before the first frame update
    void Start()
    {
        self=GetComponent<Transform>();
        hinge = GetComponent<HingeJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        //check if door can lock
        if (self.localEulerAngles.y >= (360f-9f)||(self.localEulerAngles.y >= (-9f)&&self.localEulerAngles.y <= (0f))) {
            lock_area=true;
        } else {
            lock_area = false;
        }
        //chack if handler is pulled down
        if (Handler.localEulerAngles.x <= -30f||(Handler.localEulerAngles.x <= (360f-30f)&&Handler.localEulerAngles.x > (0f))) {
            handle_down = true;
        } else{
            handle_down = false;    
        }
        //prevent close if handler up
        if (handle_down||lock_area) {
            LockCollider.gameObject.SetActive(false);
        } else {
            LockCollider.gameObject.SetActive(true);
        }
        //prevent open if handler up
        if (lock_area&&!handle_down) {
            limits.min = 0;
            hinge.limits = limits;
        } else {
            limits.min = -90;
            hinge.limits = limits;
        }
    }
}
