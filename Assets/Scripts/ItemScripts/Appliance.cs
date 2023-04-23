using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appliance : MonoBehaviour
{
    // public static event Action OnApplianceInteract; // it does it on all appliances in scene
    // public static event Action<Sprite> OnApplianceSurfaceSet; // it does it on all appliances in scene

    [SerializeField] Item myItemComponent; // a pointer to this item, maybe we dont need it but we are using it right now
    public int ItemID { get; private set; }
    public int PowerLevel { get; private set; }
    public Item ItemHeld { get; private set; } // Change to an item Data Class instead of actual item? Well see if it is more performant to do that or just keep it as is

    public SO_Appliance ApplianceData_ReadOnly { get { return GameAssets.ApplianceDataReadOnly(ItemID); } }

    private void Start()
    {
        PowerLevel = 0;
    }

    public void SetApplianceData(SO_Appliance applianceData, int itemID)
    {
        ItemID = itemID;
        if (applianceData != null)
        {
            if (applianceData.defaultHeldItem != 0)
            {
                ItemHeld = ItemManager.GenerateItem(applianceData.defaultHeldItem, gameObject.transform.localPosition);
                ItemHeld.DisableUICollider();
                // add Sprite to the item surface top
            }
        }
    }

    public void RemoveItemHeld()
    {
        ItemHeld = null;
    }

    public bool TrySetItemHeld(Item newItem)
    {
        if (ItemHeld == null)
        {
            ItemHeld = newItem;
            newItem.DisableUICollider();
            Debug.Log($"Appliance(TrySetItem) : Sucessfully set the item{newItem.Name_My} on the appliance surface");
            return true;
        }
        Debug.Log($"Appliance(TrySetItem) : Surface occupied. Unable to set the item{newItem.Name_My} on the appliance surface");
        return false;
    }

    // Maybe Delete the function below since it is a duplicate of the function above
    public bool TrySetItemSurface(Item newItem)
    {
        if (ItemHeld == null)
        {
            ItemHeld = newItem;
            newItem.DisableUICollider();
            // myItemComponent?.SetSurfaceSprite(newItem.Sprite);
            Debug.Log("Appliance(TrySetItemSurface) : Sucessfully set the item on the appliance surface");
            return true;
        }
        Debug.Log("Appliance(TrySetItemSurface) : Surface occupied. Unable to set the item on the appliance surface");
        return false;
    }



    void OnDisable()
    {
#if UNITY_EDITOR
        // Debug.Log("Appliance(OnDisable): script was disabled");
#endif
    }

    void OnEnable()
    {
#if UNITY_EDITOR
        // Debug.Log("Appliance(OnDisable): script was enabled");
#endif

    }

    public bool Interact()
    {

        return false;
    }
    
    public void Use(SO_Tool toolData, float incrementValue)
    {
        if(toolData != null)
        {
            if(ApplianceData_ReadOnly != null)
            {
                if (ApplianceData_ReadOnly.toolWhiteList.Contains(toolData.itemID))
                {
                    if(ItemHeld != null)
                    {
                        Debug.Log("Appliance(ToolUse): Process Item held by appliance");
                        switch (ItemHeld.ItemCategory_My)
                        {
                            case ItemCategory.Tool:
                                // Check if the Tool is a container
                                // Check if the container has food or something
                                // ApplyApplianceAction(toolData, ItemHeld.Tool.Contained);
                                break;
                            case ItemCategory.Food:
                                // ChopItem
                                // Add to chop progress
                                if(ItemHeld.Food.ChoppedState_curr != ItemHeld.Food.Default_Food_Data.maxChoppedState)
                                {
                                    ApplyApplianceAction(toolData, ItemHeld, incrementValue);
                                } else
                                {
                                    Debug.Log("Appliance(Use): Done");
                                }
                                break;
                        }
                    }
                }
            }
        } else
        {
            Debug.Log("Appliance(ToolUse): Tool Data not found");
        }
    }

    private void ApplyApplianceAction(SO_Tool toolData, Item itemToProcess, float incrementValue)
    {
        // It is safe to assume appliance action will always process food and no other category
        switch (ApplianceData_ReadOnly.applianceAction)
        {
            case ApplianceAction.Chop:
                if(toolData != null)
                {

                    if (toolData.toolUseType == ToolUseType.Mix)
                    {
                        Debug.Log("Appliance(ApplyApplianceAction): Mix Action");
                    }
                    else
                    {
                        Debug.Log("Appliance(ApplyApplianceAction): Chop Action");
                        // appliance increment process
                        // if process is increameneted the chop item
                        if (itemToProcess.Food.IncrementProgress(incrementValue))
                        {
                            itemToProcess.Food.ChopItem(itemToProcess, myItemComponent);
                        }
                    }
                }
                break;
            case ApplianceAction.Mix:
                break;
            case ApplianceAction.Heat:
                break;
            case ApplianceAction.Chill:
                break;
            case ApplianceAction.Freeze:
                break;
        }
    }








}
