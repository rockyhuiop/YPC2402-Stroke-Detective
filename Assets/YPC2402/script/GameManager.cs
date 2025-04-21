using HurricaneVR.Framework.Core.Utils;
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
    private string diff;

    private Dictionary<GameObject, StrokeDetectiveNPCData> NPCDataDict = new Dictionary<GameObject, StrokeDetectiveNPCData>();
    public int Correct_NPC=0;
    public int Wrong_NPC=0;
    public TMP_Text Scoreboard;
    public GameObject Finish;
    private float time_used;
    private float avg_acc;
    private float avg_time;
    [SerializeField] string level;
    // Start is called before the first frame update
    void Start()
    {
        time_used=Time.time;
        if(instance == null){
            instance = this;
        }else{
            Destroy(this);
        }
        if (GameObject.FindObjectOfType<GlobalGameManager>().Easy) {
            diff="easy";
        } else {
            diff="hard";
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
            NPCDataDict.Add(NPCs[i], NPCs[i].GetComponent<NPC>().NPCData);
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
        //time_used=Time.time-time_used;
        StartCoroutine(FinishCoroutine());
        //Finish.SetActive(true);
        //Finish.transform.GetChild(0).GetComponent<TMP_Text>().SetText(
        //    "Congratulations!\n" +
        //    "You have finished the game!\n" +
        //    "The correct rate is "+ Correct_NPC*100.0/NPCs.Length + "%\n"+
        //    "The average correct rate is "+ avg_acc + "\n"+
        //    "The time used is "+ time_used + "\n"+
        //    "The average time used is "+ avg_time
        //);
    }
    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator FinishCoroutine()
    {
        time_used=Time.time-time_used;
        String api_uri="http://y24asx4.ddns.net:5000/api/levels/";
        String uri=api_uri+"add_result";
        using (UnityWebRequest www = UnityWebRequest.Post(
            uri, 
            $"{{ " +
            $"\"level\": {level}, " +
            $"\"acc\": {Correct_NPC*100.0/NPCs.Length}, " +
            $"\"time\": {time_used}, " +
            $"\"difficulty\": \"{diff}\" " +
            $"}}",
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
                Debug.Log("Result upload complete!");
            }
        }
        uri=api_uri+"get_avg/"+level+"/"+diff;
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
                    result=result.Replace("\"", "");
                    Debug.Log(result);
                    Debug.Log(result.Split(",")[0].Split(":")[1].Trim());
                    Debug.Log(result.Split(",")[1].Split(":")[1].Trim());
                    avg_acc=float.Parse(result.Split(",")[0].Split(":")[1].Trim());
                    avg_time=float.Parse(result.Split(",")[1].Split(":")[1].Trim());
                    Debug.Log(pages[page] + ":\nReceived: " + result);
                    break;
            }
        }
        Finish.SetActive(true);
        var accuracy_row=Finish.transform.FindChildRecursive("accuracy");
        var time_row=Finish.transform.FindChildRecursive("time");
        accuracy_row.Find("you").GetComponent<TMP_Text>().SetText((Correct_NPC*100.0/NPCs.Length).ToString("F2")+"%");
        accuracy_row.Find("avg").GetComponent<TMP_Text>().SetText(avg_acc.ToString("F2")+"%");
        time_row.Find("you").GetComponent<TMP_Text>().SetText(time_used.ToString("F2")+"S");
        time_row.Find("avg").GetComponent<TMP_Text>().SetText(avg_time.ToString("F2")+"S");
        /*
        Congratulations!
        You have finished the game!
        The correct rate is 100%
        The average correct rate is 100
        The time used is 8.298458 s
        The average time used is 8.298458 s 
        */
    }
}
