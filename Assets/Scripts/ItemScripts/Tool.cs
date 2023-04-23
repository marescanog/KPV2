using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    public int ItemID { get; private set; }

    public SO_Tool ToolReadOnlyData_my { get { return GameAssets.ToolDataReadOnly(ItemID); } }

    public void SetToolData(SO_Tool toolData, int itemID)
    {
        if (toolData != null)
        {
            ItemID = toolData.itemID;
        } else
        {
            ItemID = itemID;
        }
    }

    public void Use(Item itemInFront)
    {
        Debug.Log("Tool(Use)");
    }

    public void Test_Use(Item itemInFront)
    {
        Debug.Log("Tool(Use) - Actual code put on hold");
        
        switch (itemInFront?.ItemCategory_My)
        {
            case ItemCategory.Appliance:
                if (itemInFront?.Appliance?.ItemHeld)
                {
                    Debug.Log("Tool(Use) item in front is appliance which has an item on top");
                    if (ToolReadOnlyData_my != null)
                    {
                        if (ToolReadOnlyData_my.itemFunctionRequirement.Contains(itemInFront.ItemID))
                        {
                            Debug.Log("Tool(Use) item is in the list required for tool to function");
                            switch (ToolReadOnlyData_my.toolUseType)
                            {
                                case ToolUseType.Contain:
                                    Debug.Log("Tool(Use) the function of this tool is to CONTAIN");
                                    break;
                                case ToolUseType.Chop:
                                    Debug.Log("Tool(Use) the function of this tool is to CHOP");
                                    ChopItem(itemInFront?.Appliance?.ItemHeld, itemInFront);
                                    break;
                                case ToolUseType.Mix:
                                    Debug.Log("Tool(Use) the function of this tool is to MIX");
                                    break;
                            }
                        }
                        else
                        {
                            Debug.Log("Tool(Use) item is NOT in the list required for tool to function");
                        }
                    }
                } else
                {
                    Debug.Log("Tool(Use) item in front is appliance which does not have an item on top");
                }
                break;
                /*
            case ItemCategory.Tool:
                Debug.Log("Tool(Use) item in front is tool");
                break;
                */
            default:
                Debug.Log("Tool(Use) item in front is NOT appliance");
                break;
        }

    }

    private void ChopItem(Item itemHeld, Item itemContainer)
    {
        if(itemHeld.ItemCategory_My == ItemCategory.Food)
        {
            Debug.Log("Tool(ChopItem) the item on top of the Appliance is food");
            // Modify Food
            itemHeld.Food.ChopItem(itemHeld, itemContainer);
        } else
        {
            Debug.Log("Tool(ChopItem) the item on top of the Appliance is NOT food");
        }
    }
}
