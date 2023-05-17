using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    [SerializeField] MaskedItemManager maskedRenderer;
    public int ItemID { get; private set; }
    public SO_Tool ToolReadOnlyData_my { get { return GameAssets.ToolDataReadOnly(ItemID); } }


    // We can probably refactor this into its own subclass ? yes refactor later esp when we are creating our own struct
    private List<Item> __containerItemList = new List<Item>(); // change to item Data or just item? Item for now and we will change to a struct once I implement pooling
    public float ContainerListTotalCapacity { get; private set; } // Stored Value instead of re-computing the Capacity over and over, every time you add an item in list you need to update this
    public float ContainerListCapacityPercentage { get { return ContainerListTotalCapacity / (ToolReadOnlyData_my == null ? 1 : (ToolReadOnlyData_my.maxCapacity == 0 ? 1 : ToolReadOnlyData_my.maxCapacity)); } }
    private void ContainerItemList_Push(Item newItem)
    {
        __containerItemList.Add(newItem);
        ContainerListTotalCapacity += newItem?.ItemReadOnlyData?.size ?? 1;
    }
    private Item ContainerItemList_Pop()
    {
        Item removedItem = null;
        if(__containerItemList.Count > 0)
        {
            removedItem = __containerItemList[0];
            ContainerListTotalCapacity -= removedItem?.ItemReadOnlyData?.size ?? 1;
            __containerItemList.RemoveAt(0);
        }
        return removedItem;
    }
    private void ContainerItemList_AddList(List<Item> newItemList)
    {
        foreach(Item itemToAdd in newItemList)
        {
            ContainerItemList_Push(itemToAdd);
        }
    }
    private void ContainerItemList_Empty()
    {
        __containerItemList.Clear();
        ContainerListTotalCapacity = 0;
    }


    public Item GetFromContainerList()
    {
        float prevPercent = ContainerListCapacityPercentage;
        Debug.Log($"Tool(GetFromContainerList) percent before pop {prevPercent}");
        Item returnVal = ContainerItemList_Pop();
        if (returnVal != null)
        {
            //             maskedRenderer.RemoveSprite(returnVal.Sprite_My, ContainerListCapacityPercentage);
            maskedRenderer.RemoveSprite(prevPercent);
        }
        Debug.Log($"Tool(GetFromContainerList) percent after pop {ContainerListCapacityPercentage}");
        return returnVal;
    }
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

    // no one is using this delete later cant figure it out
    public void Use(Item itemInFront)
    {
        // no one is using this delete later cant figure it out
        Debug.Log("Tool(Use)");
    }
    // no one is using this delete later cant figure it out
    public void Test_Use(Item itemInFront)
    {
        // no one is using this delete later cant figure it out
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

    public bool TryAddItemContainer(Item itemToAdd)
    {
        bool retval = false;
        // Debug.Log("Tool(TryAddItemContainer): itemToAdd is Food for now, material later");
        // Check SO Item Data if this Tool has the capability to add items to a List
        if (ToolReadOnlyData_my != null && ToolReadOnlyData_my.maxCapacity > 0)
        {
            Debug.Log("Tool(TryAddItemContainer): This tool can be used to store items");
            // SOME NOTES TO ADJUST AND IMPLEMENT LATER
            // Add data to list
            //      Check if list is full
            //          If not full then add to list
            //              How to tell between different items? chopped etc -> Item Data? Let us assume that for now it can only add Food items
            //              For now it will not stack similar items but add each item individuallly per list
            // Update UI if successful
            if(TryAddItemToContainerList(itemToAdd))
            {
                // EnableMaskRenderer(); should be done by mask renderer and not tool
                maskedRenderer.AddSprite(itemToAdd.Sprite_My, ContainerListCapacityPercentage);
                retval = true;
            }

        } else
        {
            Debug.Log("Tool(TryAddItemContainer): This tool CANNOT be used to store items");
        }
        return retval;
    }

    private bool TryAddItemToContainerList(Item itemToAdd)
    {
        bool retVal = false;
        // check if List is at Maximum Capacity
        // compute Total List capacity
        if (ContainerListTotalCapacity < ToolReadOnlyData_my.maxCapacity) // float vs int, we'll deal with that later or something, change size to int instead of float
        {
            ContainerItemList_Push(itemToAdd);
            retVal = true;
            Debug.Log($"Tool(TryAddItemContainer): Successfully added {itemToAdd.Name_My} to list.");
        } else
        {
            Debug.Log("Tool(TryAddItemContainer): 1 - This tool is FULL and CANNOT add any more items into the container");
            Debug.Log($"Tool(TryAddItemContainer): 2 - ContainerTotalCapacity {ContainerListTotalCapacity} vs ItemSize {itemToAdd?.ItemReadOnlyData?.size ?? 1}");
        }
        return retVal;
    }


    public bool TryUseWithoutAppliance(Vector3 position)
    {
        Debug.Log("Tool:TryUseWithoutAppliance(): Executed");
        bool retval = false;
        switch (ToolReadOnlyData_my.toolUseType)
        {
            case ToolUseType.Chop:
                break;
            case ToolUseType.Contain:
                Item removedItem = TryRemoveItemToContainerList();
                if (removedItem != null)
                {
                    // Enable UI Collider for the item that is dropped
                    // remove it from the mask 
                    // set new position
                    removedItem.EnableUICollider(position);
                    retval = true;
                    Debug.Log("Tool(TryUseWithoutAppliance): Sucessfully Removed Item from tool");
                }
                break;
            case ToolUseType.Flip:
                break;
            case ToolUseType.Ignite:
                break;
            case ToolUseType.Mix:
                break;
            case ToolUseType.Whip:
                break;
        }
        return retval;
    }

    private Item TryRemoveItemToContainerList()
    {
        Item retVal = null;
        if (__containerItemList.Count != 0) 
        {
            retVal = GetFromContainerList();
            if (retVal != null)
            {
                Debug.Log($"Tool(TryRemoveItemToContainerList): removed Item {retVal.Name_My}");
            }
        }
        else
        {
            Debug.Log("Tool(TryRemoveItemToContainerList): This tool has no items inside of it");
        }
        return retVal;
    }

    public void DisableMask()
    {
        maskedRenderer.enabled = false;
    }

    public void EnableMask()
    {
        maskedRenderer.enabled = true;
    }

    public MaskRendererSpriteAssets GetMaskRendererSpriteAssets()
    {
        return maskedRenderer.GenerateMaskRendererSpriteAssets();
    }

    public void SetMaskRendererSprites(MaskRendererSpriteAssets newSprites)
    {
        maskedRenderer.CopySpriteAssets(newSprites);
    }
}
