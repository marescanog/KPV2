using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Food")]
public class SO_Food : ScriptableObject
{
    // Read-only Data
    public int itemID;
    public SO_Item choppedItemResult;
    public List<Flavor> flavors = new List<Flavor>(); // items the tool needs to function
    public Sprite[] choppedSprite = new Sprite[ChoppedState.GetNames(typeof(ChoppedState)).Length]; // items the tool needs to function
    public ChoppedState maxChoppedState;
    // public SO_Food cookedItemResult;
    // public SO_Food grilledItemResult;
    // public SO_Food boiledItemResult;
}
