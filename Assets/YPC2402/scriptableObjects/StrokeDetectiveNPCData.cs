using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Data", menuName = "Game/New Stroke Detective NPC Data")]
public class StrokeDetectiveNPCData : ScriptableObject
{
    public bool isStroke;
    public string symptoms;
    public string NPCName;
    public string NPCDescription;

}