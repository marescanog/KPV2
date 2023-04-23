using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Appliance")]
public class SO_Appliance : ScriptableObject
{
    // Read-only Data

    public int itemID;
    public int maxPowerCapacity;
    public PowerType powerType;
    public ApplianceAction applianceAction;
    public CookResultType defaultCookResultType;
    public int defaultHeldItem;
    public List<int> itemWhiteList = new List<int>();
    public List<int> toolWhiteList = new List<int>();
}
