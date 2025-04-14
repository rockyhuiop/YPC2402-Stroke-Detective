using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UMA.PoseTools;
using UnityEngine;

public class ExpressionControl : MonoBehaviour
{
    private UMAExpressionPlayer ExpressionScript;
    private Dictionary<string, FieldInfo> parameterFields; // Cache for parameter fields
    private Dictionary<string, Coroutine> activeOscillations = new Dictionary<string, Coroutine>(); // Track active oscillations

    void Start()
    {
        ExpressionScript = GetComponent<UMAExpressionPlayer>();
        parameterFields = new Dictionary<string, FieldInfo>();
        foreach (var field in typeof(UMAExpressionPlayer).GetFields())
        {
            if (field.FieldType == typeof(float))
            {
                parameterFields[field.Name] = field;
            }
        }
    }

    void Update()
    {
        if (ExpressionScript == null)
        {
            ExpressionScript = GetComponent<UMAExpressionPlayer>();
        }
    }

    // Set a parameter value dynamically
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

    // Coroutine to oscillate a parameter over time
    private IEnumerator OscillateParameter(string paramName, float min, float max, float frequency)
    {
        float time = 0f;
        while (true)
        {
            time += Time.deltaTime;
            float value = min + (max - min) * (Mathf.Sin(time * frequency * 2 * Mathf.PI) + 1) / 2;
            SetParameter(paramName, value);
            //Debug.Log($"Oscillating {paramName}: current value = {value}"); // Log oscillation value
            yield return null;
        }
    }

    // Apply commands from the second chatbot
    public void ApplyExpressionCommands(Dictionary<string, object> commands)
    {
        foreach (var kvp in commands)
        {
            string paramName = kvp.Key;
            object command = kvp.Value;

            if (command is float value)
            {
                if (activeOscillations.TryGetValue(paramName, out var coroutine))
                {
                    StopCoroutine(coroutine);
                    activeOscillations.Remove(paramName);
                    Debug.Log($"Stopped oscillation for {paramName}"); // Log oscillation stop
                }
                SetParameter(paramName, value);
                Debug.Log($"Set {paramName} to {value}"); // Log static value
            }
            else if (command is OscillateCommand oscillate)
            {
                if (activeOscillations.TryGetValue(paramName, out var existingCoroutine))
                {
                    StopCoroutine(existingCoroutine);
                    Debug.Log($"Stopped existing oscillation for {paramName}"); // Log previous oscillation stop
                }
                var newCoroutine = StartCoroutine(OscillateParameter(paramName, oscillate.min, oscillate.max, oscillate.frequency));
                activeOscillations[paramName] = newCoroutine;
                Debug.Log($"Started oscillation for {paramName}: min={oscillate.min}, max={oscillate.max}, frequency={oscillate.frequency}"); // Log oscillation start
            }
        }
    }

    // Existing methods
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