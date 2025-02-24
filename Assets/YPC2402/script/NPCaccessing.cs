using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA.CharacterSystem;

public class NPCaccessing : MonoBehaviour
{
    private DynamicCharacterAvatar UMA;
    // Start is called before the first frame update
    void Start()
    {
        UMA=GetComponent<DynamicCharacterAvatar>();
    }
    /* to get all the valid item */
    //UMA=GetComponent<DynamicCharacterAvatar>();
    //foreach (var slot in UMA.AvailableRecipes) {
    //    foreach (var recipe in UMA.AvailableRecipes[slot.Key]) {
    //        Debug.Log(slot.Key+" "+recipe.name);
    //    }
    //}
    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeClothe(string slot, string item) {
        UMA.SetSlot(slot,item);
        UMA.BuildCharacter();
    }
    public void ChangeClotheRand(string slot) {
        UMA.SetSlot(slot,UMA.AvailableRecipes[slot][Random.Range(0,UMA.AvailableRecipes[slot].Count-1)].name);
        UMA.BuildCharacter();
    }
}
