using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UMA.PoseTools;
using Unity.VisualScripting;
using UnityEngine;

public class ExpressionControl : MonoBehaviour
{
    private UMAExpressionPlayer ExpressionScript;
    private Animator animator;
    private Dictionary<string, FieldInfo> parameterFields;
    private Dictionary<string, Coroutine> activeOscillations = new Dictionary<string, Coroutine>();

    private string baseState;

    void Start()
    {
        ExpressionScript = GetComponent<UMAExpressionPlayer>();
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator component not found.");
        }
        parameterFields = new Dictionary<string, FieldInfo>();
        foreach (var field in typeof(UMAExpressionPlayer).GetFields())
        {
            if (field.FieldType == typeof(float))
            {
                parameterFields[field.Name] = field;
            }
        }
        baseState = GetComponent<PoseControl>().GetPose();
    }

    void Update()
    {
        if (ExpressionScript == null)
        {
            ExpressionScript = GetComponent<UMAExpressionPlayer>();
        }
    }

    

    private void SetParameter(string paramName, float value)
    {
        if (parameterFields.TryGetValue(paramName, out var field))
        {
            field.SetValue(ExpressionScript, value);
        }
        else
        {
            Debug.LogWarning($"Parameter {paramName} not found.");
        }
    }

    private IEnumerator OscillateParameter(string paramName, float min, float max, float frequency)
    {
        float time = 0f;
        while (true)
        {
            time += Time.deltaTime;
            float value = min + (max - min) * (Mathf.Sin(time * frequency * 2 * Mathf.PI) + 1) / 2;
            SetParameter(paramName, value);
            yield return null;
        }
    }

    public void ApplyExpressionCommands(Dictionary<string, object> commands)
    {
        PoseControl poseControl = GetComponent<PoseControl>();
        foreach (var kvp in commands)
        {
            Debug.Log($"Processing command: {kvp.Key} = {kvp.Value}");
            if (kvp.Key == "animation_trigger")
            {
                if (kvp.Value is string triggerName)
                {
                    if (triggerName == "lift_arm_full" || triggerName == "lift_arm_half")
                    {
                        
                        if (baseState != "unknown")
                        {
                            string specificTrigger = "liftarm_" + baseState;
                            if (triggerName == "lift_arm_half")
                            {
                                specificTrigger += "_half";
                            }
                            poseControl.SetPose(specificTrigger);
                            Debug.Log($"Set specific animation trigger: {specificTrigger} based on {triggerName} and state {baseState}");
                        }
                        else
                        {
                            Debug.LogWarning("Unknown base state, cannot set lift arm animation.");
                        }
                    }
                    else
                    {
                        poseControl.SetPose(triggerName);
                        Debug.Log($"Set animation trigger: {triggerName}");
                    }
                }
            }
            else
            {
                string paramName = kvp.Key;
                object command = kvp.Value;
                if (command is float value)
                {
                    if (activeOscillations.TryGetValue(paramName, out var coroutine))
                    {
                        StopCoroutine(coroutine);
                        activeOscillations.Remove(paramName);
                        Debug.Log($"Stopped oscillation for {paramName}");
                    }
                    SetParameter(paramName, value);
                    Debug.Log($"Set {paramName} to {value}");
                }
                else if (command is OscillateCommand oscillate)
                {
                    if (activeOscillations.TryGetValue(paramName, out var existingCoroutine))
                    {
                        StopCoroutine(existingCoroutine);
                        Debug.Log($"Stopped existing oscillation for {paramName}");
                    }
                    var newCoroutine = StartCoroutine(OscillateParameter(paramName, oscillate.min, oscillate.max, oscillate.frequency));
                    activeOscillations[paramName] = newCoroutine;
                    Debug.Log($"Started oscillation for {paramName}: min={oscillate.min}, max={oscillate.max}, frequency={oscillate.frequency}");
                }
            }
        }
    }

    public void FullySmile()
    {
        ExpressionScript.rightMouthSmile_Frown = 1.0f;
        ExpressionScript.leftMouthSmile_Frown = 1.0f;
        ExpressionScript.leftLowerLipUp_Down = -1.0f;
        ExpressionScript.rightLowerLipUp_Down = -1.0f;
    }

    public void HalfSmile(bool isLeft)
    {
        if (isLeft)
        {
            ExpressionScript.leftMouthSmile_Frown = 1.0f;
            ExpressionScript.leftLowerLipUp_Down = -1.0f;
        }
        else
        {
            ExpressionScript.rightMouthSmile_Frown = 1.0f;
            ExpressionScript.rightLowerLipUp_Down = -1.0f;
        }
    }
}