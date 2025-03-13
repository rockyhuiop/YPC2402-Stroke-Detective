using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    //list of all NPC
    [SerializeField] GameObject[] NPCs;
    [SerializeField] StrokeDetectiveNPCData[] NPCData;

    private Dictionary<GameObject, StrokeDetectiveNPCData> NPCDataDict = new Dictionary<GameObject, StrokeDetectiveNPCData>();
    public int Correct_NPC=0;
    public int Wrong_NPC=0;
    public TMP_Text Scoreboard;
    public GameObject Finish;
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null){
            instance = this;
        }else{
            Destroy(this);
        }


        Scoreboard = GameObject.FindGameObjectWithTag("Scoreboard").transform.GetChild(0).GetComponent<TMP_Text>();
        Finish = GameObject.FindGameObjectWithTag("Finish");
        Finish.SetActive(false);
        UpdateScoreboard();
        InitializeDictionary();
        foreach (var NPC in NPCs) {
            NPCaccessing npcacc=NPC.GetComponent<NPCaccessing>();
            npcacc.ChangeClotheRandOld();
        }
    }

    private void InitializeDictionary(){
        for (int i = 0; i < NPCData.Length; i++)
        {
            NPCDataDict.Add(NPCs[i], NPCData[i]);
        }
    }


    public void DetermineStroke(bool answer, GameObject NPC, GameObject correctSign, GameObject wrongSign) {
        if (NPCDataDict[NPC].isStroke == answer) {
            correctSign.SetActive(true);
            AddDone(true);
        } else {
            wrongSign.SetActive(true);
            AddDone(false);
        }
    }



    
    public void AddDone(bool isCorrect) {
        if (isCorrect) {
            Correct_NPC++;
            Debug.Log("Correct");
        } else {
            Wrong_NPC++;
            Debug.Log("Wrong");
        }
        UpdateScoreboard();
        if (NPCs.Length - Correct_NPC - Wrong_NPC <= 0) {
            SetFinish();
        }
        
    }
    void UpdateScoreboard() {
        Scoreboard.SetText(
            "NPC left: " + (NPCs.Length-Correct_NPC-Wrong_NPC).ToString() + "\n" +
            "Correct: " + (Correct_NPC).ToString() + "\n" +
            "Wrong: " + (Wrong_NPC).ToString() + "\n" );
    }
    void SetFinish() {
        Finish.SetActive(true);
        Finish.transform.GetChild(0).GetComponent<TMP_Text>().SetText(
            "Congratulations!\n" +
            "You have finished the game!\n" +
            "The correct rate is "+ Correct_NPC*100.0/NPCs.Length + "%");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
