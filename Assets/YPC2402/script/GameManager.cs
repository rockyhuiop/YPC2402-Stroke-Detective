using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


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
    private float avg_acc;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UploadResult(0));
        if(instance == null){
            instance = this;
        }else{
            Destroy(this);
        }


        Scoreboard = GameObject.FindGameObjectWithTag("Scoreboard").transform.GetChild(0).GetComponent<TMP_Text>();
        Finish = GameObject.FindGameObjectWithTag("Finish");
        NPCs = GameObject.FindGameObjectsWithTag("NPC");
        Finish.SetActive(false);
        UpdateScoreboard();
        InitializeDictionary();
        foreach (var NPC in NPCs) {
            NPCaccessing npcacc=NPC.GetComponent<NPCaccessing>();
            npcacc.ChangeClotheRandOld();
        }
    }

    private void InitializeDictionary(){
        for (int i = 0; i < NPCs.Length; i++)
        {
            NPCDataDict.Add(NPCs[i], NPCData[UnityEngine.Random.Range(0,NPCData.Length-1)]);
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
            "The correct rate is "+ Correct_NPC*100.0/NPCs.Length + "%\n"+
            "The average correct rate is "+ avg_acc
        );
        StartCoroutine(UploadResult(Correct_NPC*100.0/NPCs.Length));
    }
    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator UploadResult(double acc)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(
            "http://localhost:5000/api/level1/add_result", 
            $"{{ \"NPC\": [1, 0, 1], \"acc\": {acc.ToString()}, \"time\": 56.5 }}",
            "application/json"
            ))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
        String uri="http://localhost:5000/api/level1/get_avg";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    String result=webRequest.downloadHandler.text;
                    result=result.Replace("{", "");
                    result=result.Replace("}", "");
                    result=result.Split(":")[1].Trim();
                    avg_acc = float.Parse(result);
                    Debug.Log(pages[page] + ":\nReceived: " + result);
                    break;
            }
        }
    }
}
