using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA.CharacterSystem;
using UnityEngine.TextCore.LowLevel;

public class NPCaccessing : MonoBehaviour
{
    private DynamicCharacterAvatar UMA;
    private bool requestChangeClotheRandOld=false;
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
        if (UMALoaded()) {
            if (requestChangeClotheRandOld) {
                string[] slots={"Chest","Legs"};
                foreach (var slot in slots) {
                    ChangeClotheRand(slot);
                }
                requestChangeClotheRandOld=false;
            }
        }
    }
    public bool UMALoaded() {
        if (UMA.WardrobeRecipes.Count > 0) {
            return true;
        } else {
            return false;
        }
    }
    public void ChangeClothe(string slot, string item) {
        UMA.SetSlot(slot,item);
        UMA.BuildCharacter();
    }
    public void ChangeClotheRand(string slot) {
        UMA.SetSlot(slot,UMA.AvailableRecipes[slot][Random.Range(0,UMA.AvailableRecipes[slot].Count-1)].name);
        UMA.BuildCharacter();
    }
    public void ChangeClotheRandOld() {
        requestChangeClotheRandOld=true;

    }
}
