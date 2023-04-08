using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Item ItemHandItem { get; private set; } // _equipslot_itemHand_left
    public Item ToolHandItem { get; private set; } //_equipslot_toolHand_right
    RaycastHit2D[] m_Results = new RaycastHit2D[5];
    // To add Inventory List Dictionary

    public static event Action<Sprite> OnInventoryItemPickUp;
    public static event Action OnInventoryItemDrop; 

    public Item UnequipItem()
    {
        Item oldItem = ItemHandItem;
        ItemHandItem = null;
        return oldItem;
    }

    public Item EquipItem(Item newItem)
    {
        Item oldItem = ItemHandItem;
        ItemHandItem = newItem;
        return oldItem;
    }

    public bool TryEquipItem(Item newItem)
    {
        if(newItem != null) // rewrite later, combine with if below and delete console logs
        {
            if (ItemHandItem == null)
            {
                ItemHandItem = newItem;
                newItem.DisableUICollider();
                Debug.Log("Inventory(TryEquipItem) : Sucessfully equipped the item into your inventory");
                return true;
            }
            Debug.Log("Inventory(TryEquipItem) : Item hand full. Unable to equip the item into your inventory");
        }else
        {
            Debug.Log("Inventory(TryEquipItem) : New Item is NULL. Cannot equip NULL into your inventory");
        }

        return false;
    }


    public bool DropItem(Item itemInFront, Vector3 playerPosition, Vector2 playerFaceDirection, float playerReach)
    {
        bool isSuccess = false;

        if (itemInFront)
        {
            Debug.Log("Inventory(DropItem): 1.1 - item is in front of player");

            isSuccess = itemInFront.TrySetItemSurface(ItemHandItem);


            /*
                if appliance has tool attached (bowl or whatever)
                        if players current item is food
                            add to appliance (bowl or whatever)
                        if players current item is material
                            if fuel is allowed to be added to appliance
                                add fuel
                if appliance has no tool attached
                        if players current item is tool
                            if tool is allowed as an attachment to the appliance
                                Attach tool
                        if players current item is food
                            if appliance allows food to be added straight
                                add food 
            */
            // Appliance.ItemHeld



            /*
            if (ItemHandItem != null)
            {
                if (itemInFront.Appliance != null && itemInFront.Appliance.isActiveAndEnabled) 
                {
                    // can drop if it is a wall and appliance is a sign
                    switch (ItemHandItem.ItemCategory)
                    {
                        case ItemCategory.Food:
                            // load into appliance or tool
                            Debug.Log("Inventory(DropItem): Item in front is Appliance while player's equipped item is Food");
                            if (itemInFront.Appliance.ItemHeld == null)
                            {
                                isSuccess = itemInFront.Appliance.TrySetItemSurface(ItemHandItem);
                                if (isSuccess)
                                {
                                    ItemHandItem = null;
                                }
                            }
                            break;
                        case ItemCategory.Tool:
                            // load into appliance
                            Debug.Log("Inventory(DropItem): Item in front is Appliance while player's equipped item is Tool");
                            break;
                        case ItemCategory.Material:
                            // fuel appliance
                            Debug.Log("Inventory(DropItem): Item in front is Appliance while player's equipped item is Material");
                            break;
                    }
                }

                else if (itemInFront.Tool != null && itemInFront.Tool.isActiveAndEnabled) 
                {
                    switch (ItemHandItem.ItemCategory)
                    {
                        case ItemCategory.Appliance:
                            // Do nothing
                            break;
                        case ItemCategory.Food:
                            // Check item In front (Appliance, Tool) 
                            break;
                        case ItemCategory.Tool:
                            // Check item In front (Appliance, Food)
                            break;
                        case ItemCategory.Material:
                            // Check item In front (Appliance, Tool?)
                            break;
                    }
                }

            }
            */
        }
        else
        {
            Debug.Log("Inventory(DropItem): 1.2 - item is not in front of player");
            // check if there is a wall or something else in front of the player where we cant put item
            int hits = Physics2D.RaycastNonAlloc(playerPosition, playerFaceDirection, m_Results, playerReach);
            Debug.Log($"RaycastNonAlloc hits {hits}");
            if (hits <= 1)
            {
                Debug.Log("Can Drop Item");
                float offsetDrop = 1.2f; //refactor and clean later
                Vector3 offSetDropposition = new Vector3(playerFaceDirection.x* offsetDrop, playerFaceDirection.y * offsetDrop, 0);
                ItemHandItem.EnableUICollider(playerPosition+ offSetDropposition);
                isSuccess = true;
            } 
        }

        if (isSuccess)
        {
            ItemHandItem = null;
            OnInventoryItemDrop?.Invoke(); // Clear Canvas HUD
        }

        return isSuccess;
    }

    public bool PickupItem(Item itemInFront, bool buttonHeld = false)
    {
        bool isSuccess = false;
        Sprite spriteToEquip = itemInFront?.Sprite_My;

        if (itemInFront)
        {
            Debug.Log("Inventory(PickupItem): 2.1 - item is in front of player");
            if (buttonHeld)
            {
                // check if item is appliance
                Debug.Log("Inventory(PickupItem): Hold Button is pressed");

                if (itemInFront?.Appliance?.isActiveAndEnabled == true)
                {
                    // Add appliance function TryEquipAppliance -> Make sure appliance is off? or if appliance can be equipped while running, write in ondisable code?

                    /*
                    if (itemInFront.Appliance.ItemHeld != null)
                    {
                        Debug.Log("Player(OnPickupHold) : You are trying to pick up an appliance that has an item on its surface. Funtionality and rules for this has not been implemented yet.");
                        // Verdict can pick up cause some appliances will have tools on them
                        // Bug where the item image does not show the surface item on the player carrying item
                    } else
                    {
          
                    }
                    */
                    Debug.Log("Player(OnPickupHold) : You are picking up an appliance");

                    isSuccess = TryEquipItem(itemInFront); // returns a bool true if equipped and false if not
                } 
            }
            else
            {
                // check if item is not appliance
                Debug.Log("Inventory(PickupItem): Button is tapped");

                // Maybe change to switch statement with item categories?
                if (itemInFront?.Food?.isActiveAndEnabled == true)
                {
                    Debug.Log("Player(PickupItem) : You are picking up Food");

                    isSuccess = TryEquipItem(itemInFront); // returns a bool true if equipped and false if not
                }

                if (itemInFront?.Tool?.isActiveAndEnabled == true)
                {
                    Debug.Log("Player(PickupItem) : You are picking up a tool");

                    isSuccess = TryEquipItem(itemInFront); // returns a bool true if equipped and false if not
                }

                if (itemInFront?.Appliance?.isActiveAndEnabled == true)
                {
                    Item applianceItemHeld = itemInFront.Appliance.ItemHeld;
                    Debug.Log($"Player(PickupItem) : You are picking the item({applianceItemHeld?.Name_My??"NULL"}) that is on the appliance({itemInFront?.Name_My ?? "NULL"})");
                    isSuccess = TryEquipItem(applianceItemHeld);
                    if (isSuccess)
                    {
                        itemInFront.Appliance.RemoveItemHeld();
                        itemInFront.RemoveSurfaceSprite();
                        spriteToEquip = applianceItemHeld.Sprite_My;
                    }
                }
            }

            if (isSuccess)
            {
                if (spriteToEquip != null)
                {
                    OnInventoryItemPickUp?.Invoke(spriteToEquip);
                }
            }
        }



        return isSuccess;
    }
}
