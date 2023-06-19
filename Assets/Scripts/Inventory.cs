using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static event Action<Sprite, MaskRendererSpriteAssets> OnInventoryItemPickUp;
    public static event Action OnInventoryItemDrop;
    public static event Action<Sprite> OnInventoryToolEquip;
    public static event Action OnInventoryToolDrop;

    public Item ItemHandItem { get; private set; } // _equipslot_itemHand_left
    public Item ToolHandItem { get; private set; } //_equipslot_toolHand_right
    RaycastHit2D[] m_Results = new RaycastHit2D[5];

    private List<Item> _itemList = new List<Item>();
    private List<Item> _toolList = new List<Item>();
    public List<Item> ItemList { get; private set; } // _equipslot_itemHand_left
    public List<Item> ToolList { get; private set; } // _equipslot_toolHand_right
    
    private int _maxInventoryCapacity = 4; // Rucksack
    // private int _maxToolCapacity = 2; // Toolbelt

    private void Awake()
    {
        ItemList = _itemList;
        ToolList = _toolList;
    }
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

                // if the item is a tool, then enable
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
        MaskRendererSpriteAssets maskRendererAssets = null;
        if (itemInFront)
        {
            Debug.Log("Inventory(PickupItem): 2.1 - item is in front of player");
            if (buttonHeld)
            {
                // check if item is appliance
                Debug.Log("Inventory(PickupItem): [buttonHeld] Hold Button is pressed");

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
                    Debug.Log("Inventory(PickupItem) : [buttonHeld] You are picking up an appliance");

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
                    Debug.Log("Inventory(PickupItem) : You are picking up Food");

                    isSuccess = TryEquipItem(itemInFront); // returns a bool true if equipped and false if not

                    if (isSuccess)
                    {
                        // maskRendererAssets = null;
                    }
                }

                if (itemInFront?.Tool?.isActiveAndEnabled == true)
                {
                    Debug.Log("Inventory(PickupItem) : You are picking up a tool");

                    isSuccess = TryEquipItem(itemInFront); // returns a bool true if equipped and false if not

                    if (isSuccess)
                    {
                        itemInFront.Tool?.DisableMask();
                        maskRendererAssets = itemInFront.GetMaskRendererSpriteAssets();
                    }
                }

                if (itemInFront?.Appliance?.isActiveAndEnabled == true)
                {
                    bool isFromContainer;
                    // Item applianceItemHeld = itemInFront.Appliance.ItemHeld;
                    Item applianceItemHeld = itemInFront.GetFromItemHeld(out isFromContainer);
                    Debug.Log($"Inventory(PickupItem) : You are picking the item({applianceItemHeld?.Name_My??"NULL"}) that is on the appliance({itemInFront?.Name_My ?? "NULL"})");

                    if (!isFromContainer)
                    {
                        isSuccess = TryEquipItem(applianceItemHeld);
                    } else
                    {
                        Debug.Log($"Inventory(PickupItem): TODO Get from container will be done at a later point");
                        // remove this if statement and just place as is
                        // when making the functionality to get items out from container
                    }


                    if (isSuccess)
                    {
                        // Get mask sprite data of the item on top of appliance
                        if (!isFromContainer)
                        {
                            maskRendererAssets = applianceItemHeld.GetMaskRendererSpriteAssets();
                        }
                        else
                        {
                            // remove this if statement and just place as is
                            // when making the functionality to get items out from container
                        }

                        // Remove Appliance Item's Sprite (Surface Sprite & Mask Renderer sprite) & Data
                        if (!isFromContainer)
                        {
                            itemInFront.RemoveSurfaceSprite();
                            itemInFront.Appliance.RemoveItemHeld();
                        }

                        if (!isFromContainer)
                        {
                            spriteToEquip = applianceItemHeld.Sprite_My;
                        } else
                        {
                            // remove this if statement and just place as is
                            // when making the functionality to get items out from container
                        }
         
                    }
                }
            }

            if (isSuccess)
            {
                if (spriteToEquip != null)
                {
                    OnInventoryItemPickUp?.Invoke(spriteToEquip, maskRendererAssets);
                }
            }
        }



        return isSuccess;
    }

    public bool ToggleEquippedItemHand()
    {
        bool isSuccess = false;

        /*
            What do you want accomplished?
            If Player has item on HandItem
                If Item List is not full
                    Add to end of list
            If Player does not have item on HandItem
                If ItemList has an item
                    Remove Item from the start of List
        */


        // GET ITEM 
        if(ItemHandItem == null)
        {
            Debug.Log("Inventory(ToggleEquippedItemHand): ItemHand is empty. Pull Item from Inventory list.");

            if(ItemList.Count != 0)
            {
                if (TryEquipItem(ItemList[0]))
                {
                    ItemList.RemoveAt(0);
                    OnInventoryItemPickUp?.Invoke(ItemHandItem.Sprite_My, ItemHandItem.GetMaskRendererSpriteAssets());
                    isSuccess = true;
                }          
            } else
            {
                Debug.Log("Inventory(ToggleEquippedItemHand): Cannot pull item from empty list.");
            }
        }
        // STORE ITEM 
        else
        {
            Debug.Log("Inventory(ToggleEquippedItemHand): ItemHand has item. Place Item into Inventory list.");
            // Check if item is an appliance, cannot add appliances to inventory?
            // Check if appliance is tool? Seperate list?

            // Debug.Log($"ItemListCount {ItemList.Count} < InventoryCap {_maxInventoryCapacity} == {ItemList.Count < _maxInventoryCapacity}");
            if (ItemList.Count < _maxInventoryCapacity)
            {
                if(ItemHandItem.ItemCategory_My != ItemCategory.Appliance)
                {
                    ItemList.Add(ItemHandItem);
                    ItemHandItem = null;
                    OnInventoryItemDrop?.Invoke(); // Clear Canvas HUD
                    isSuccess = true;
                } else
                {
                    Debug.Log("Inventory(ToggleEquippedItemHand): Cannot place Appliance Item into Inventory List.");
                    // Maybe swap items like tools swap ? 
                }
            }
            else
            {
                // Debug.Log("Inventory(ToggleEquippedItemHand): Cannot place item into Full Inventory List.");
            }
        }

        return isSuccess;
    }

    public bool ToggleEquippedToolHand()
    {
        bool isSuccess = false;

        // GET TOOL (from Toolbelt) 

        if(ItemHandItem == null)
        {
            Debug.Log("Inventory(ToggleEquippedItemHand): ItemHand is empty. Pull Item from Inventory list.");

            if(ToolList.Count != 0)
            {
                /*
                if (TryEquipItem(ToolList[0])) // Change Try Equip Tool? 
                {
                    ToolList.RemoveAt(0);
                    OnInventory ItemPickUp ?.Invoke(ItemHandItem.Sprite_My);
                    isSuccess = true;
                }   
                */
            }
            else
            {
                Debug.Log("Inventory(ToggleEquippedItemHand): Cannot pull item from empty list.");
            }
        }
        /*
            // STORE TOOL (from Toolbelt) 
            else
            {
                Debug.Log("Inventory(ToggleEquippedItemHand): ItemHand has item. Place Item into Inventory list.");
                // Check if item is an appliance, cannot add appliances to inventory?
                // Check if appliance is tool? Seperate list?

                // Debug.Log($"ItemListCount {ItemList.Count} < InventoryCap {_maxInventoryCapacity} == {ItemList.Count < _maxInventoryCapacity}");
                if (ItemList.Count < _maxInventoryCapacity)
                {
                    if(ItemHandItem.ItemCategory_My != ItemCategory.Appliance)
                    {
                        ItemList.Add(ItemHandItem);
                        ItemHandItem = null;
                        OnInventoryItemDrop?.Invoke(); // Clear Canvas HUD
                        isSuccess = true;
                    } else
                    {
                        // Debug.Log("Inventory(ToggleEquippedItemHand): Cannot place Appliance Item into Inventory List.");
                    }
                }
                else
                {
                    // Debug.Log("Inventory(ToggleEquippedItemHand): Cannot place item into Full Inventory List.");
                }
            } 
        */

        return isSuccess;
    }

    public void ToggleFetchTool()
    {
        if (ItemHandItem == null || (ItemHandItem.Tool != null && ItemHandItem.Tool?.isActiveAndEnabled == true))
        {
            Debug.Log("Inventory(ToggleFetchTool): Swapping Itemhand and Tool");
            SwapItemHandWithToolHand();
        }

        if(ItemHandItem != null)
        {
            if (!(ItemHandItem.Tool != null && ItemHandItem.Tool?.isActiveAndEnabled == true))
            {
                Debug.Log("Inventory(ToggleFetchTool): Cannot Swap Itemhand with Non Tool");
            }
        }
    }

    /*
    public bool TryEquipIntoToolHand(Item toolToSet)
    {
        bool retVal = false;
        if (ToolHandItem == null)
        {
            ToolHandItem = toolToSet;
            OnInventoryToolEquip?.Invoke(toolToSet.Sprite_My);
            retVal = true;
        } else
        {
            Debug.Log("Inventory(ToggleFetchTool): Cannot Set Tool. The Tool hand is full");
        }
        return retVal;
    }
    */

    private void SwapItemHandWithToolHand()
    {
        Debug.Log("Inventory(SwapItemHandWithToolHand)");

        Item temp = ItemHandItem;

        // Debug.Log($"Before Swap: ItemHand: {ItemHandItem?.Name_My??"NULL"} - ToolHand: {ToolHandItem?.Name_My??"NULL"}");
        
        // Transfer Tool to ItemHand
        if (ToolHandItem == null)
        {
            ItemHandItem = null;
            OnInventoryItemDrop?.Invoke(); 
        }
        else
        {
            ItemHandItem = ToolHandItem;
            OnInventoryItemPickUp?.Invoke(ToolHandItem.Sprite_My, ToolHandItem.GetMaskRendererSpriteAssets());
        }

        // Transfer Tool to ToolHand
        if (temp == null)
        {
            ToolHandItem = null;
            OnInventoryToolDrop?.Invoke();
        } else
        {
            ToolHandItem = temp;
            OnInventoryToolEquip?.Invoke(temp.Sprite_My);
        }

        // Debug.Log($"After Swap: ItemHand:{ItemHandItem?.Name_My ?? " NULL"} - ToolHand{ToolHandItem?.Name_My ?? " NULL"}");

    }

}
