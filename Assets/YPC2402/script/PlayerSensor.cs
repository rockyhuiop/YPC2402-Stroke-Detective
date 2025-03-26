using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    private NavAI navAI;
    public List<GameObject> colliderList = new List<GameObject>();
    private void Start() {
        navAI = GetComponent<Transform>().parent.GetComponent<NavAI>();
    }
    public void OnTriggerEnter(Collider collider)
    {
        if (!colliderList.Contains(collider.gameObject))
        {
            colliderList.Add(collider.gameObject);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (colliderList.Contains(collider.gameObject))
        {
            colliderList.Remove(collider.gameObject);
        }
    }

    public List<GameObject> AllGameObject()
    {
        return colliderList;
    }
    private void Update() {
        bool havePlayer=false;
        foreach (var collider in colliderList) {
            if (collider.transform.CompareTag("Player")) {
                havePlayer=true;
                break;
            }          
        }
        navAI.force_stop=havePlayer;
    }
}
