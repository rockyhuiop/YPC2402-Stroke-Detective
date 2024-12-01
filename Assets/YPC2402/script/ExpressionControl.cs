using System.Collections;
using System.Collections.Generic;
using UMA.PoseTools;
using UnityEngine;

public class ExpressionControl : MonoBehaviour
{
    private UMAExpressionPlayer ExpressionScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //find the ExpressionScript if it is null (the ExpressionScript is not apper in the beginning, if set it in start(), it will be missing)
        if ( ExpressionScript == null ) {
            ExpressionScript = GetComponent<UMAExpressionPlayer>();
        } else {
            
            HalfSmile(true);
        }
    }
    //smile normally
    public void FullySmile() {
        ExpressionScript.rightMouthSmile_Frown=1.0f;
        ExpressionScript.leftMouthSmile_Frown=1.0f;
        ExpressionScript.leftLowerLipUp_Down=-1.0f;
        ExpressionScript.rightLowerLipUp_Down=-1.0f;
    }
    //smile but just either left or right (having stroke)
    public void HalfSmile(bool isLeft) {
        //left half smile
        if (isLeft) {
            ExpressionScript.leftMouthSmile_Frown=1.0f;
            ExpressionScript.leftLowerLipUp_Down=-1.0f;
        }
        //right half smile
        else
        {
            ExpressionScript.rightMouthSmile_Frown=1.0f;
            ExpressionScript.rightLowerLipUp_Down=-1.0f;
        }
    }

}
