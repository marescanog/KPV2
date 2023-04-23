
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Tool")]
public class SO_Tool : ScriptableObject
{
    // Read-only Data
    public int itemID;
    // public List<int> itemWhiteList = new List<int>(); // items the tool can accept
    public List<int> itemFunctionRequirement = new List<int>(); // items the tool needs to function ??
    // public List<int> applianceAttach = new List<int>(); // items the tool can attach to
    public int maxCapacity; // if 0 then it is not a vessel for storing items
    public ToolUseType toolUseType;
}