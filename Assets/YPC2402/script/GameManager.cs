using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //list of all NPC
    [SerializeField] GameObject[] NPCs;
    public int Correct_NPC=0;
    public int Wrong_NPC=0;
    public TMP_Text Scoreboard;
    public GameObject Finish;
    // Start is called before the first frame update
    void Start()
    {
        Scoreboard = GameObject.FindGameObjectWithTag("Scoreboard").transform.GetChild(0).GetComponent<TMP_Text>();
        Finish = GameObject.FindGameObjectWithTag("Finish");
        Finish.SetActive(false);
        UpdateScoreboard();
    }
    public void AddDone(bool isCorrect) {
        if (isCorrect) {
            Correct_NPC++;
        } else {
            Wrong_NPC++;
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
