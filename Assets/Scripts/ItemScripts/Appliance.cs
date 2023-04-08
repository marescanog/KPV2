using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appliance : MonoBehaviour
{
    // public static event Action OnApplianceInteract; // it does it on all appliances in scene
    // public static event Action<Sprite> OnApplianceSurfaceSet; // it does it on all appliances in scene

    [SerializeField] Item myItemComponent; // a pointer, maybe we dont need it
    public int ItemID { get; private set; }
    public int PowerLevel { get; private set; }
    public Item ItemHeld { get; private set; } // Change to an item Data Class instead of actual item? Well see if it is more performant to do that or just keep it as is

   
    private void Start()
    {
        PowerLevel = 0;
    }

    public void SetApplianceData(SO_Appliance applianceData)
    {
        if(applianceData!= null)
        {
            ItemID = applianceData.itemID;
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
        Debug.Log("Appliance(OnDisable): script was disabled");
#endif
    }

    void OnEnable()
    {
#if UNITY_EDITOR
        Debug.Log("Appliance(OnDisable): script was enabled");
#endif

    }

    public bool Interact()
    {

        return false;
    }
    
}
